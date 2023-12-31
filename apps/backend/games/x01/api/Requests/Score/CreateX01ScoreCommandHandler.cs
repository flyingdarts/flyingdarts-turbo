public record CreateX01ScoreCommandHandler(
    IDynamoDbService DynamoDbService, 
    IAmazonApiGatewayManagementApi ApiGatewayClient,
    CachingService<X01State> CachingService,
    X01MetadataService MetadataService,
    ConnectionService ConnectionService) : IRequestHandler<CreateX01ScoreCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(CreateX01ScoreCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<CreateX01ScoreCommand>
        {
            Action = "games/x01/score",
            Message = request
        };

        // Update connectionId
        await ConnectionService.UpdateConnectionIdAsync(request.PlayerId, request.ConnectionId, cancellationToken);

        // Load game state
        await CachingService.Load(request.GameId, cancellationToken);

        // Create a dart record
        var setsAndLegs = GetSetsAndLegs();
        if (setsAndLegs != null) 
        {
            var gameDart = GameDart.Create(long.Parse(request.GameId), request.PlayerId, request.Input, request.Score, setsAndLegs.Item1, setsAndLegs.Item2);

            // Write dart to database
            await DynamoDbService.WriteGameDartAsync(gameDart, cancellationToken);

            // Write dart to cache
            CachingService.AddDart(gameDart);

            // Save new state
            await CachingService.Save(cancellationToken);
        }

        // Get metadata for clients
        socketMessage.Metadata = (await MetadataService.GetMetadata(request.GameId, cancellationToken)).toDictionary();

        // Game was finished
        if (socketMessage.Metadata["WinningPlayer"] != null)
        { // create game record
            var game = (await DynamoDbService.ReadGameAsync(long.Parse(request.GameId), cancellationToken)).Single();

            // update game to match state
            game.Status = GameStatus.Finished;

            // Write game to database
            await DynamoDbService.WriteGameAsync(game, cancellationToken);

            // Write game to cache
            CachingService.AddGame(game);

            // Save new state
            await CachingService.Save(cancellationToken);
        }

        // Notify people in the room
        await NotifyRoomAsync(socketMessage, CachingService.State.Users.Select(x => x.ConnectionId).ToArray(), cancellationToken);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private Tuple<int, int> GetSetsAndLegs()
    {
        var currentSet = CachingService.State.Darts.Select(x => x.Set).DefaultIfEmpty(1).Max(); 
        var currentLeg = CachingService.State.Darts.Select(x => x.Leg).DefaultIfEmpty(1).Max();

        if (CachingService.State.Darts.Any() && CachingService.State.Darts.OrderBy(x => x.CreatedAt).Last().GameScore == 0)
        {
            currentLeg++;
            if (currentLeg > CachingService.State.Game.X01.Legs)
            {
                currentLeg = 1;
                currentSet++;
            }
        }

        return new Tuple<int, int>(currentSet, currentLeg);
    }

    public async Task NotifyRoomAsync(SocketMessage<CreateX01ScoreCommand> request, string[] connectionIds, CancellationToken cancellationToken)
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