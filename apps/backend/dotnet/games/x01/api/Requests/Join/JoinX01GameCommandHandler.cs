using Flyingdarts.Connection.Services;
using Flyingdarts.Metadata.Services.Services.X01;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.Join;

public record JoinX01GameCommandHandler(
    IDynamoDbService DynamoDbService,
    IAmazonApiGatewayManagementApi ApiGatewayClient,
    CachingService<X01State> CachingService,
    X01MetadataService MetadataService,
    ConnectionService ConnectionService
) : IRequestHandler<JoinX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(
        JoinX01GameCommand request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var socketMessage = new SocketMessage<JoinX01GameCommand>
        {
            Action = "games/x01/join",
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

        // Update user connection ID in cache
        await UpdateUserConnectionIdAsync(
            request.PlayerId,
            request.ConnectionId,
            cancellationToken
        );

        // Keep track of game in request
        request.Game = CachingService.State.Game;

        // Add player to game if not already present
        await AddPlayerToGameAsync(request, cancellationToken);

        // Start game if we have 2 players
        await StartGameIfReadyAsync(request, cancellationToken);

        // Populate metadata as the final step
        socketMessage.Metadata = (
            await MetadataService.GetMetadataAsync(
                request.Game.GameId.ToString(),
                cancellationToken
            )
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

    private async Task UpdateUserConnectionIdAsync(
        string playerId,
        string connectionId,
        CancellationToken cancellationToken
    )
    {
        var existingUser = CachingService.State.Users.FirstOrDefault(x => x.UserId == playerId);
        if (existingUser is not null)
        {
            existingUser.ConnectionId = connectionId;
            await CachingService.Save(cancellationToken);
        }
    }

    private async Task AddPlayerToGameAsync(
        JoinX01GameCommand request,
        CancellationToken cancellationToken
    )
    {
        if (CachingService.State.Players.Any(x => x.PlayerId == request.PlayerId))
            return;

        var player = GamePlayer.Create(long.Parse(request.GameId), request.PlayerId);
        await DynamoDbService.WriteGamePlayerAsync(player, cancellationToken);
        CachingService.AddPlayer(player);

        var user = await DynamoDbService.ReadUserAsync(player.PlayerId, cancellationToken);
        CachingService.AddUser(user);
        await CachingService.Save(cancellationToken);
    }

    private async Task StartGameIfReadyAsync(
        JoinX01GameCommand request,
        CancellationToken cancellationToken
    )
    {
        if (CachingService.State.Players.Count != 2)
            return;

        request.Game!.Status = GameStatus.Started;
        request.Game!.LSI1 = $"{GameStatus.Started}#{request.Game!.GameId}";
        request.Game!.SortKey = $"{request.Game!.GameId}#{GameStatus.Started}";

        await DynamoDbService.WriteGameAsync(request.Game, cancellationToken);
        CachingService.AddGame(request.Game);
        await CachingService.Save(cancellationToken);
    }

    private async Task NotifyRoomAsync(
        SocketMessage<JoinX01GameCommand> request,
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
