namespace Flyingdarts.Backend.Signalling.OnDisconnect.CQRS;

public class OnDisconnectCommandHandler : IRequestHandler<OnDisconnectCommand, APIGatewayProxyResponse>
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;
    public OnDisconnectCommandHandler(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
        _tableName = Environment.GetEnvironmentVariable("TableName");
    }
    public async Task<APIGatewayProxyResponse> Handle(OnDisconnectCommand request, CancellationToken cancellationToken)
    {
        var ddbRequest = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "ConnectionId", new AttributeValue { S = request.ConnectionId } }
            }
        };

        await _dynamoDb.DeleteItemAsync(ddbRequest, cancellationToken);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = "Disconnected."
        };
    }
}