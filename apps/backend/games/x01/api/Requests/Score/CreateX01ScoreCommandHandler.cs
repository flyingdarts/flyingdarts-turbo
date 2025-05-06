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
            var game = (await DynamoDbService.ReadStartedGameAsync(long.Parse(request.GameId), cancellationToken)).Single();

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
        var darts = CachingService.State.Darts;
        int currentSet = 1; // Start from set 1
        int currentLeg = 1;
        int legsNeededToWinSet = (CachingService.State.Game.X01.Legs + 1) / 2; // (+1)/2 Because its bestOf mode
        int setsNeededToWinGame = (CachingService.State.Game.X01.Sets + 1) / 2; // (+1)/2 Because its bestOf mode

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