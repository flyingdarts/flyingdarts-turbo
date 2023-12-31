using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

public class OnConnectCommandHandler : IRequestHandler<OnConnectCommand, APIGatewayProxyResponse>
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;
    public OnConnectCommandHandler(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
        _tableName = System.Environment.GetEnvironmentVariable("TableName");
    }
    public async Task<APIGatewayProxyResponse> Handle(OnConnectCommand request, CancellationToken cancellationToken)
    {

        if (request != null)
            await CreateSignallingRecord(request.ConnectionId);
        
        return new APIGatewayProxyResponse
        {
            StatusCode = 201,
            Body = JsonSerializer.Serialize(request)
        };
    }
    private async Task CreateSignallingRecord(string connectionId)
    {
        var ddbRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "ConnectionId", new AttributeValue{ S = connectionId }}
            }
        };

        await _dynamoDb.PutItemAsync(ddbRequest);
    }
}