using System;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Flyingdarts.Backend.User.Profile.Api.Response;

namespace Flyingdarts.Backend.User.Profile.Api.Requests.Update;

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
        try
        {
            var user = await GetUserAsync(request.UserId, cancellationToken);

            user.ConnectionId = request.ConnectionId;
            user.Profile.Country = request.Country;
            user.Profile.Email = request.Email;
            user.Profile.UserName = request.UserName;

            var userWrite = _dbContext.CreateBatchWrite<Persistence.User>(_options.Value.ToOperationConfig());

            userWrite.AddPutItem(user);

            await userWrite.ExecuteAsync(cancellationToken);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new UserProfileDto
                {
                    Country = user.Profile.Country,
                    Email = user.Profile.Email,
                    UserName = user.Profile.UserName
                }),
                Headers = new Dictionary<string, string>() {
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
                StatusCode = 400,
                Body = JsonSerializer.Serialize(ex),
                Headers = new Dictionary<string, string>() {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                }
            };
        }
    }

    private async Task<Persistence.User> GetUserAsync(string userId, CancellationToken cancellationToken) 
    {
        var results = await _dbContext.FromQueryAsync<Persistence.User>(QueryConfig(userId), _options.Value.ToOperationConfig()).GetRemainingAsync(cancellationToken);

        return results.Single();
    }
    private static QueryOperationConfig QueryConfig(string userId) 
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FD#USER");
        queryFilter.AddCondition("LSI1", QueryOperator.BeginsWith, userId);
        return new QueryOperationConfig { Filter = queryFilter };
    }
}