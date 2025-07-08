using Flyingdarts.Backend.Games.X01.Queue.Models;
using Flyingdarts.Metadata.Services.Services.X01;

namespace Flyingdarts.Backend.Games.X01.Queue.CQRS;

public class HandleX01QueueCommandHandler : IRequestHandler<HandleX01QueueCommand>
{
    private readonly CachingService<X01State> _cachingService;
    private readonly IAmazonApiGatewayManagementApi _apiGatewayClient;
    private readonly IDynamoDbService _dynamoDbService;
    private readonly QueueService<X01Queue> _queueService;
    private readonly X01MetadataService _metadataService;

    public HandleX01QueueCommandHandler(
        CachingService<X01State> cachingService, 
        IDynamoDbService dynamoDbService, 
        IAmazonApiGatewayManagementApi apiGatewayClient, 
        QueueService<X01Queue> queueService,
        X01MetadataService metadataService)
    {
        _cachingService = cachingService;
        _apiGatewayClient = apiGatewayClient;
        _dynamoDbService = dynamoDbService;
        _queueService = queueService;
        _metadataService = metadataService;
    }

    public async Task Handle(HandleX01QueueCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Look for match
        var records = await _queueService.GetRecords(cancellationToken);
        var match = FindMatch(records, request.Owner.PlayerId);

        // If match found, handle it
        if (match is not null)
        {
            Console.WriteLine("Match found");
            
            var settings = match.X01;
            var playerIds = records.Select(x => x.PlayerId).ToArray();
            var users = await _dynamoDbService.ReadUsersAsync(playerIds, cancellationToken);
            var connectionIds = users.Select(x => x.ConnectionId).ToArray();
            
            Console.WriteLine("Players " + string.Join(" ", playerIds));
            Console.WriteLine("ConnectionIds " + string.Join(" ", connectionIds));
            
            // Creates game and game player objects
            await InitializeGameAsync(settings, playerIds, cancellationToken);

            // Sends response to clients
            await HandleResponseBackAsync(connectionIds, cancellationToken);

            await _queueService.DeleteRecords(records, cancellationToken);
        }
    }

    private static X01Queue? FindMatch(List<X01Queue> records, string playerId)
    {
        return records.FirstOrDefault(x => x.PlayerId != playerId);
    }

    private async Task HandleResponseBackAsync(string[] connectionIds, CancellationToken cancellationToken)
    {
        var message = new SocketMessage<HandleQueueResponse>
        {
            Action = "games/x01/queue",
            Message = new()
            {
                GameId = _cachingService.State.Game.GameId.ToString(),
            },
        };

        // Populate metadata as the final step
        message.Metadata = (await _metadataService.GetMetadataAsync(_cachingService.State.Game.GameId.ToString(), cancellationToken)).ToDictionary();

        await NotifyRoomAsync(message, connectionIds, cancellationToken);
    }

    private async Task InitializeGameAsync(X01GameSettings settings, string[] playerIds, CancellationToken cancellationToken)
    {
        // Create game record
        var game = Game.Create(2, settings);

        // Update game to match state
        game.Status = GameStatus.Started;
        game.LSI1 = $"{GameStatus.Started}#{game.GameId}";
        game.SortKey = $"{game.GameId}#{GameStatus.Started}";

        // Write game record to database
        await _dynamoDbService.WriteGameAsync(game, cancellationToken);

        // Initialize state in cache
        _cachingService.State = X01State.Create(game.GameId);
        _cachingService.AddGame(game);
        await _cachingService.Save(cancellationToken);

        // Create player records for the game
        foreach (var playerId in playerIds)
        {
            // Create game player
            var player = GamePlayer.Create(game.GameId, playerId);

            // Write game player to database
            await _dynamoDbService.WriteGamePlayerAsync(player, cancellationToken);

            // Write game player to cache
            _cachingService.AddPlayer(player);

            // Get user for game player
            var user = await _dynamoDbService.ReadUserAsync(player.PlayerId, cancellationToken);

            // Add user to cache
            _cachingService.AddUser(user);
        }

        // Persist changes to cache
        await _cachingService.Save(cancellationToken);
    }

    private async Task NotifyRoomAsync(SocketMessage<HandleQueueResponse> request, string[] connectionIds, CancellationToken cancellationToken)
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
            Console.WriteLine("Sending websocket message to: " + connectionId);
            await _apiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
        }
    }
}