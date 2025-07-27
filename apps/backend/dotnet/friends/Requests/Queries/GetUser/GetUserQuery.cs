namespace Flyingdarts.Backend.Friends.Api.Requests.Queries.GetUser;

public class GetUserQuery : IRequest<APIGatewayProxyResponse>
{
    public string UserId { get; set; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, APIGatewayProxyResponse>
{
    private readonly IDynamoDbService DynamoDbService;

    public GetUserQueryHandler(IDynamoDbService dynamoDbService)
    {
        DynamoDbService = dynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var user = await DynamoDbService.ReadUserByAuthProviderUserIdAsync(
                request.UserId,
                cancellationToken
            );

            var userDto = new UserDto { UserId = user.UserId };

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(userDto),
                Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                }
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { error = ex.Message }),
                Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                }
            };
        }
    }
}

public class UserDto
{
    public string UserId { get; set; }
}
