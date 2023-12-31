public record JoinX01QueueCommandHandler(
    IDynamoDBContext DbContext,
    ConnectionService ConnectionService) : IRequestHandler<JoinX01QueueCommand, APIGatewayProxyResponse>
{
    public async Task<APIGatewayProxyResponse> Handle(JoinX01QueueCommand request, CancellationToken cancellationToken)
    {
        var socketMessage = new SocketMessage<JoinX01QueueCommand>
        {
            Action = "games/x01/joinqueue",
            Message = request
        };

        await ConnectionService.UpdateConnectionIdAsync(request.PlayerId, request.ConnectionId, cancellationToken);

        var queueState = X01Queue.Create(request.PlayerId, -1, request.ConnectionId, X01GameSettings.Create(request.Sets, request.Legs));

        var queueWrite = DbContext.CreateBatchWrite<X01Queue>(OperationConfig);

        queueWrite.AddPutItem(queueState);
        await queueWrite.ExecuteAsync(cancellationToken);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    private DynamoDBOperationConfig OperationConfig
    {
        get
        {
            var tableName = $"Flyingdarts-X01Queue-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}";
            return new DynamoDBOperationConfig { OverrideTableName = tableName };
        }
    }
}