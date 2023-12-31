using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IOptions<ApplicationOptions> _options;

    public UpdateUserProfileCommandHandler(IDynamoDBContext DbContext, IOptions<ApplicationOptions> ApplicationOptions)
    {
        _dbContext = DbContext;
        _options = ApplicationOptions;
    }
    public async Task<APIGatewayProxyResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(request.UserId, cancellationToken);

        user.ConnectionId = request.ConnectionId;
        user.Profile.Country = request.Country;
        user.Profile.Email = request.Email;
        user.Profile.UserName = request.UserName;

        var userWrite = _dbContext.CreateBatchWrite<User>(_options.Value.ToOperationConfig()); 
        
        userWrite.AddPutItem(user);

        await userWrite.ExecuteAsync(cancellationToken);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new UserProfileDto
            {
                UserId = user.UserId,
                Country = user.Profile.Country,
                Email = user.Profile.Email,
                UserName = user.Profile.UserName
            }),
            Headers = new Dictionary<string, string>() {
                    { "Access-Control-Allow-Headers", "Content-Type" },
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,POST" }
                }
        };
    }

    private async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken) 
    {
        var results = await _dbContext.FromQueryAsync<User>(QueryConfig(userId), _options.Value.ToOperationConfig()).GetRemainingAsync(cancellationToken);

        return results.Single();
    }
    private static QueryOperationConfig QueryConfig(string userId) 
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FD#USER");
        queryFilter.AddCondition("SK", QueryOperator.BeginsWith, userId);
        return new QueryOperationConfig { Filter = queryFilter };
    }
}