using System.Linq;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Flyingdarts.Persistence;
using Flyingdarts.Shared;
using MediatR;
using Microsoft.Extensions.Options;
using Flyingdarts.Backend.Shared.Models;

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
        var queryItems = await _dbContext.FromQueryAsync<User>(QueryConfig(request.CognitoUserName), _applicationOptions.ToOperationConfig())
            .GetRemainingAsync(cancellationToken);
        var socketMessage = new SocketMessage<GetUserProfileResponse>();
        socketMessage.Action = "v2/user/profile/get";
        // Handle the query results
        if (queryItems != null && queryItems.Any())
        {
            // At least one item was found
            // Use the fetched object(s) as needed
            var result = queryItems.First();
            socketMessage.Message = GetUserProfileResponse.From(result, result.Profile);

        }


        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };

    }
    private static QueryOperationConfig QueryConfig(string userName)
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.User);
        queryFilter.AddCondition("LSI1", QueryOperator.BeginsWith, userName);
        return new QueryOperationConfig { Filter = queryFilter };
    }
    class GetUserProfileResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

        public static GetUserProfileResponse From(User user, UserProfile userProfile)
        {
            return new GetUserProfileResponse
            {
                UserId = user.UserId.ToString(),
                UserName = userProfile.UserName,
                Email = userProfile.Email,
                Country = userProfile.Country
            };
        }
    }
}