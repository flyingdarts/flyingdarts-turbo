using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Flyingdarts.Backend.Shared.Models;
using Flyingdarts.Backend.Shared.Services;
using Flyingdarts.Persistence;
using MediatR;
using System.Text;
using System.Text.Json;

namespace Flyingdarts.Backend.Games.X01.Queue.CQRS;
class Range
{
    public int Start { get; }
    public int End { get; }

    public Range(int start, int end)
    {
        Start = start;
        End = end;
    }

    public bool IsInRange(int value)
    {
        return value >= Start && value <= End;
    }
}
public class HandleX01QueueCommandHandler : IRequestHandler<HandleX01QueueCommand>
{
    private readonly CachingService<X01State> CachingService;
    private readonly IAmazonApiGatewayManagementApi ApiGatewayClient;
    private readonly IDynamoDbService DynamoDbService;
    private readonly X01MetadataService MetadataService;
    private readonly QueueService<X01Queue> QueueService;
    public HandleX01QueueCommandHandler(CachingService<X01State> cachingService, X01MetadataService metadataService, IDynamoDbService dynamoDbService, IAmazonApiGatewayManagementApi apiGatewayClient, QueueService<X01Queue> queueService)
    {
        CachingService = cachingService;
        ApiGatewayClient = apiGatewayClient;
        DynamoDbService = dynamoDbService;
        MetadataService = metadataService;
        QueueService = queueService;    
    }
    public async Task Handle(HandleX01QueueCommand request, CancellationToken cancellationToken)
    {
        // Look for match
        var records = await QueueService.GetRecords(cancellationToken);

        var match = FindMatch(records, request.Owner.PlayerId);

        // if match found
        if (match != null)
        {
            // get game settings
            var settings = match.X01;
            var playerIds = records.Select(x=>x.PlayerId).ToArray();
            var connectionIds = records.Select(x => x.ConnectionId).ToArray();

            // Creates game and game player objects
            await InitializeGame(settings, playerIds, cancellationToken);

            // Sends response to clients
            await HandleResponseBack(connectionIds, cancellationToken);

            await QueueService.DeleteRecords(records, cancellationToken);
        }
    }
    private X01Queue FindMatch(List<X01Queue> records, string playerId)
    {
        foreach (var record in records.Where(x=>x.PlayerId != playerId))
        {
            if (record.PlayerId != playerId)
            {
                return record;
            }
        }

        throw new Exception("No match found");
    }
    private async Task HandleResponseBack(string[] connectionIds, CancellationToken cancellationToken)
    {
        var message = new SocketMessage<HandleQueueResponse>
        {
            Action = "games/x01/queue",
            Message = new()
            {
                GameId = CachingService.State.Game.GameId.ToString(),
            },
        };

        await NotifyRoomAsync(message, connectionIds, cancellationToken);
    }

    private async Task InitializeGame(X01GameSettings settings, string[] playerIds, CancellationToken cancellationToken)
    {
        // create game record
        var game = Game.Create(2, settings);

        // update game to match state
        game.Status = GameStatus.Started;
        game.LSI1 = $"{GameStatus.Started}#{game!.GameId}";
        game.SortKey = $"{game.GameId}#{GameStatus.Started}";

        // write game record to database
        await DynamoDbService.WriteGameAsync(game, cancellationToken);

        // initialize state in cache
        CachingService.State = X01State.Create(game.GameId);
        CachingService.AddGame(game);

        await CachingService.Save(cancellationToken);

        // Create player records for the game
        foreach (var playerId in playerIds)
        {
            // create game player
            var player = GamePlayer.Create(game.GameId, playerId);

            // write game player to database
            await DynamoDbService.WriteGamePlayerAsync(player, cancellationToken);

            // write game player to cache
            CachingService.AddPlayer(player);

            // get user for game player
            var user = await DynamoDbService.ReadUserAsync(player.PlayerId, cancellationToken);

            // add user to cache
            CachingService.AddUser(user);
        }

        // persist changes to cache
        await CachingService.Save(cancellationToken);
    }

    public async Task NotifyRoomAsync(SocketMessage<HandleQueueResponse> request, string[] connectionIds, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)));

        foreach (var connectionId in connectionIds)
        {
            if (!string.IsNullOrEmpty(connectionId))
            {
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
}

public class HandleQueueResponse
{
    public string GameId { get; set; }
}