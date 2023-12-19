using Amazon.ApiGatewayManagementApi.Model;
using Flyingdarts.Backend.Shared.Models;
using Flyingdarts.Persistence;
using MediatR;
using System.Text.Json;
using System.Text;
using Flyingdarts.Backend.Shared.Services;
using Amazon.ApiGatewayManagementApi;
using Amazon.SQS;
using Amazon.SQS.Model;

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
    public HandleX01QueueCommandHandler(CachingService<X01State> cachingService, X01MetadataService metadataService, IDynamoDbService dynamoDbService, IAmazonApiGatewayManagementApi apiGatewayClient)
    {
        CachingService = cachingService;
        ApiGatewayClient = apiGatewayClient;
        DynamoDbService = dynamoDbService;
        MetadataService = metadataService;
    }
    public async Task Handle(HandleX01QueueCommand request, CancellationToken cancellationToken)
    {
        X01Queue FindMatchingQueue(List<X01Queue> records, int targetAverage)
        {
            foreach (var x01Queue in records)
            {
                if (x01Queue.Average == targetAverage)
                {
                    return x01Queue;
                }
            }

            // Return null if no match is found
            return null;
        }

        X01Queue FindMatchingQueueByRange(List<X01Queue> records, int targetAverage, List<Range> ranges)
        {
            foreach (var x01Queue in records)
            {
                foreach (var range in ranges)
                {
                    if (range.IsInRange(x01Queue.Average) && x01Queue.Average == targetAverage)
                    {
                        return x01Queue;
                    }
                }
            }

            // Return null if no match is found within the specified range
            return null;
        }

        // average ranges
        List<Range> ranges = new List<Range> {
            new Range (10, 19),
            new Range(20,29),
            new Range(30,39),
            new Range(40,49),
            new Range(50,59),
            new Range(60,69),
            new Range(70,79),
            new Range(80,89),
            new Range(90,99)
        };

        // Look for match
        X01Queue? match = FindMatchingQueueByRange(request.Records, request.Owner.Average, ranges);

        // if match found
        if (match != null)
        {
            // get game settings
            var settings = match.X01;
            var playerIds = new[] { match.PlayerId, request.Owner.PlayerId };

            // Creates game and game player objects
            await InitializeGame(settings, playerIds, cancellationToken);

            // Sends response to clients
            await HandleResponseBack(cancellationToken);
        }
    }
    private async Task HandleResponseBack(CancellationToken cancellationToken)
    {
        var message = new SocketMessage<HandleQueueResponse>
        {
            Action = "games/x01/queue",
            Message = new()
            {
                GameId = CachingService.State.Game.GameId.ToString(),
            },
            Metadata = (await MetadataService.GetMetadata(CachingService.State.Game.GameId.ToString(), cancellationToken)).toDictionary()
        };

        await NotifyRoomAsync(message, cancellationToken);
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
        CachingService.CreateInitial(X01State.Create(game.GameId), game);

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

    public async Task NotifyRoomAsync(SocketMessage<HandleQueueResponse> request, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)));

        foreach (var user in CachingService.State.Users)
        {
            if (!string.IsNullOrEmpty(user.ConnectionId))
            {
                var connectionId = user.ConnectionId;

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