using Flyingdarts.Connection.Services;
using Flyingdarts.Metadata.Services.Services.X01;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.Score;

public record CreateX01ScoreCommandHandler(
    IDynamoDbService DynamoDbService,
    IAmazonApiGatewayManagementApi ApiGatewayClient,
    CachingService<X01State> CachingService,
    X01MetadataService MetadataService,
    ConnectionService ConnectionService
) : IRequestHandler<CreateX01ScoreCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(
        CreateX01ScoreCommand request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var socketMessage = new SocketMessage<CreateX01ScoreCommand>
        {
            Action = "games/x01/score",
            Message = request
        };

        // Update connection ID
        await ConnectionService.UpdateConnectionIdAsync(
            request.PlayerId,
            request.ConnectionId,
            cancellationToken
        );

        // Load game state
        await CachingService.Load(request.GameId, cancellationToken);

        // Create and save dart record
        await CreateDartRecordAsync(request, cancellationToken);

        // Check if game is finished and update accordingly
        await HandleGameCompletionAsync(request.GameId, cancellationToken);

        // Populate metadata as the final step
        socketMessage.Metadata = (
            await MetadataService.GetMetadataAsync(request.GameId, cancellationToken)
        ).ToDictionary();

        // Notify people in the room
        await NotifyRoomAsync(
            socketMessage,
            CachingService.State.Users.Select(x => x.ConnectionId).ToArray(),
            cancellationToken
        );

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private async Task CreateDartRecordAsync(
        CreateX01ScoreCommand request,
        CancellationToken cancellationToken
    )
    {
        var setsAndLegs = GetSetsAndLegs();
        if (setsAndLegs is null)
            return;

        var gameDart = GameDart.Create(
            long.Parse(request.GameId),
            request.PlayerId,
            request.Input,
            request.Score,
            setsAndLegs.Value.Set,
            setsAndLegs.Value.Leg
        );

        // Write dart to database
        await DynamoDbService.WriteGameDartAsync(gameDart, cancellationToken);

        // Write dart to cache
        CachingService.AddDart(gameDart);

        // Save new state
        await CachingService.Save(cancellationToken);
    }

    private async Task HandleGameCompletionAsync(string gameId, CancellationToken cancellationToken)
    {
        // Get metadata to check if game is finished
        var metadata = await MetadataService.GetMetadataAsync(gameId, cancellationToken);

        if (metadata.WinningPlayer is not null)
        {
            var game = (
                await DynamoDbService.ReadStartedGameAsync(long.Parse(gameId), cancellationToken)
            ).Single();
            game.Status = GameStatus.Finished;

            // Write game to database
            await DynamoDbService.WriteGameAsync(game, cancellationToken);

            // Write game to cache
            CachingService.AddGame(game);

            // Save new state
            await CachingService.Save(cancellationToken);
        }
    }

    private (int Set, int Leg)? GetSetsAndLegs()
    {
        var darts = CachingService.State.Darts;
        if (darts is { Count: 0 })
            return null;

        int currentSet = 1;
        int currentLeg = 1;
        int legsNeededToWinSet = (CachingService.State.Game.X01.Legs + 1) / 2;
        int setsNeededToWinGame = (CachingService.State.Game.X01.Sets + 1) / 2;

        // Track leg wins per player per set
        var legWinsPerPlayer = new Dictionary<string, int>();

        foreach (var dart in darts)
        {
            if (!legWinsPerPlayer.ContainsKey(dart.PlayerId))
            {
                legWinsPerPlayer[dart.PlayerId] = 0;
            }

            // Check if the player has won the leg
            if (dart.GameScore == 0)
            {
                legWinsPerPlayer[dart.PlayerId]++;

                // Check if the player has won enough legs to win the set
                if (legWinsPerPlayer[dart.PlayerId] >= legsNeededToWinSet)
                {
                    currentSet++;
                    currentLeg = 1; // Reset leg count for the new set

                    // Reset leg wins for all players for the new set
                    foreach (var playerId in legWinsPerPlayer.Keys.ToList())
                    {
                        legWinsPerPlayer[playerId] = 0;
                    }
                }
                else
                {
                    currentLeg++; // Move to the next leg within the same set
                }
            }
        }

        return (currentSet, currentLeg);
    }

    private async Task NotifyRoomAsync(
        SocketMessage<CreateX01ScoreCommand> request,
        string[] connectionIds,
        CancellationToken cancellationToken
    )
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)));

        foreach (var connectionId in connectionIds)
        {
            if (string.IsNullOrEmpty(connectionId))
                continue;

            var postConnectionRequest = new PostToConnectionRequest
            {
                ConnectionId = connectionId,
                Data = stream
            };

            stream.Position = 0;
            await ApiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
        }
    }
}
