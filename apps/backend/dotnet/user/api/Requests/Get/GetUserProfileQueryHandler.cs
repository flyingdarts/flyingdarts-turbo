using System;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Flyingdarts.Backend.User.Profile.Api.Response;

namespace Flyingdarts.Backend.User.Profile.Api.Requests.Get;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly ApplicationOptions _applicationOptions;
    public GetUserProfileQueryHandler(IDynamoDBContext dbContext, IOptions<ApplicationOptions> applicationOptions)
    {
        _dbContext = dbContext;
        _applicationOptions = applicationOptions.Value;
    }
    public async Task<APIGatewayProxyResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var queryItems = await _dbContext.FromQueryAsync<Persistence.User>(QueryConfig(request.AuthProviderUserId), _applicationOptions.ToOperationConfig())
                .GetRemainingAsync(cancellationToken);

            // Handle the query results
            if (queryItems != null && queryItems.Any())
            {
                // At least one item was found
                // Use the fetched object(s) as needed
                var result = queryItems.First();

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(new UserProfileDto
                    {
                        UserId = result.UserId,
                        Country = result.Profile.Country,
                        Email = result.Profile.Email,
                        UserName = result.Profile.UserName,
                    }),
                    Headers = new Dictionary<string, string>() {
                        { "Access-Control-Allow-Origin", "*" },
                        { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                        { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                    }
                };
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = 404,
                Headers = new Dictionary<string, string>() {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                }
            };
        }
        catch(Exception ex)
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
    private static QueryOperationConfig QueryConfig(string userName)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FD#USER");
        queryFilter.AddCondition("LSI1", QueryOperator.BeginsWith, userName);
        return new QueryOperationConfig { Filter = queryFilter };
    }
}