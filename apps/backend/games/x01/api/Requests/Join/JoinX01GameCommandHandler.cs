using Flyingdarts.Backend.Games.X01.Services.Metadata;

public record JoinX01GameCommandHandler(
    IDynamoDbService DynamoDbService, 
    IAmazonApiGatewayManagementApi ApiGatewayClient, 
    CachingService<X01State> CachingService, 
    X01MetadataService MetadataService,
    ConnectionService ConnectionService) : IRequestHandler<JoinX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(JoinX01GameCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<JoinX01GameCommand>
        {
            Action = "games/x01/join",
            Message = request
        };

        // Update connectionId
        Console.WriteLine(socketMessage);
        Console.WriteLine(request);
        await ConnectionService.UpdateConnectionIdAsync(request.PlayerId, request.ConnectionId, cancellationToken);
        
        // Load game state
        await CachingService.Load(request.GameId, cancellationToken);

        if (CachingService.State.Users.Any(x=>x.UserId == request.PlayerId))
            CachingService.State.Users.Single(x=>x.UserId== request.PlayerId).ConnectionId = request.ConnectionId;
        await CachingService.Save(cancellationToken);

        // Keep track of gamn in request, doesn't make sense i know
        request.Game = CachingService.State.Game;
        
        // Player joins game
        if (!CachingService.State.Players.Any(x => x.PlayerId == request.PlayerId))
        {
            var player = GamePlayer.Create(long.Parse(request.GameId), request.PlayerId);

            await DynamoDbService.WriteGamePlayerAsync(player, cancellationToken);
                
            CachingService.AddPlayer(player);

            var user = await DynamoDbService.ReadUserAsync(player.PlayerId, cancellationToken);

            CachingService.AddUser(user);

            await CachingService.Save(cancellationToken);
        }
        
        // Game started  with 2 players
        if (CachingService.State.Players.Count == 2)
        {
            request.Game!.Status = GameStatus.Started;
            request.Game!.LSI1 = $"{GameStatus.Started}#{request.Game!.GameId}";
            request.Game!.SortKey = $"{request.Game!.GameId}#{GameStatus.Started}";

            await DynamoDbService.WriteGameAsync(request.Game, cancellationToken);

            CachingService.AddGame(request.Game!);

            await CachingService.Save(cancellationToken);
        }

        // Get metadata for clients
        socketMessage.Metadata = (await MetadataService.GetMetadata(request.Game.GameId.ToString(), cancellationToken)).toDictionary();       

        // Notify people in the room
        await NotifyRoomAsync(socketMessage, CachingService.State.Users.Select(x=>x.ConnectionId).ToArray(),  cancellationToken);
         
        return new APIGatewayProxyResponse { 
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage) 
        };
    }
    public async Task NotifyRoomAsync(SocketMessage<JoinX01GameCommand> request, string[] connectionIds, CancellationToken cancellationToken)
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
