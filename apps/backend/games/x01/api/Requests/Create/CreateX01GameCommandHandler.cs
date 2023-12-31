public record CreateX01GameCommandHandler(
    IDynamoDBContext DbContext, 
    CachingService<X01State> CachingService,
    ConnectionService ConnectionService) : IRequestHandler<CreateX01GameCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(CreateX01GameCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<CreateX01GameCommand>
        {
            Action = "games/x01/create",
            Message = request
        };

        await ConnectionService.UpdateConnectionIdAsync(request.PlayerId, request.ConnectionId, cancellationToken);

        var game = await CreateGame(request.Sets, request.Legs, cancellationToken);

        socketMessage.Message!.GameId = game.GameId.ToString();


        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private async Task<Game> CreateGame(int sets, int legs, CancellationToken cancellationToken)
    {
        var game = Game.Create(2, X01GameSettings.Create(sets, legs));

        CachingService.State = X01State.Create(game.GameId);
        CachingService.AddGame(game);
        await CachingService.Save(cancellationToken);

        var gameWrite = DbContext.CreateBatchWrite<Game>(OperationConfig);
        gameWrite.AddPutItem(game);

        await gameWrite.ExecuteAsync(cancellationToken);
        return game;
    }
    
    private DynamoDBOperationConfig OperationConfig
    {
        get
        {
            var tableName = $"Flyingdarts-Application-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}";
            return new DynamoDBOperationConfig { OverrideTableName = tableName };
        }
    }
}