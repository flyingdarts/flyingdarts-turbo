using Flyingdarts.Meetings.Service.Services;

namespace Flyingdarts.Backend.Signalling.OnConnect.CQRS;

public class OnConnectCommandEnsureUserHasMeetingRoomHandler : IRequestPostProcessor<OnConnectCommand, APIGatewayProxyResponse>
{
    private readonly IMeetingService _meetingService;
    private readonly IDynamoDbService _dynamoDbService;
    public OnConnectCommandEnsureUserHasMeetingRoomHandler(IMeetingService meetingService, IDynamoDbService dynamoDbService)
    {
        _meetingService = meetingService ?? throw new ArgumentNullException();
        _dynamoDbService = dynamoDbService ?? throw new ArgumentNullException();
    }
    public async Task Process(OnConnectCommand request, APIGatewayProxyResponse response, CancellationToken cancellationToken)
    {
        var user = await _dynamoDbService.ReadUserAsync(request.UserId, cancellationToken);
        var meeting = await _meetingService.GetByNameAsync(request.UserId, cancellationToken);
        if (meeting is null)
        {
            meeting = await _meetingService.CreateAsync(user.UserId, cancellationToken);

            if (meeting is null)
            {
                throw new Exception("Couldn't create the meeting for some reason");
            }
            
            user.MeetingIdentifier = meeting.Id.ToString();
            
            await _dynamoDbService.WriteUserAsync(user, cancellationToken);
        }
    }
}
public class OnConnectCommandHandler : IRequestHandler<OnConnectCommand, APIGatewayProxyResponse>
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;
    public OnConnectCommandHandler(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
        _tableName = Environment.GetEnvironmentVariable("TableName");
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