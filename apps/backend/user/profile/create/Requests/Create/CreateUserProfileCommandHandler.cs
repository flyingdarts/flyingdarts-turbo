using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using MediatR;
using Microsoft.Extensions.Options;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IOptions<ApplicationOptions> _applicationOptions;

    public CreateUserProfileCommandHandler(IDynamoDBContext DbContext, IOptions<ApplicationOptions> ApplicationOptions)
    {
        _dbContext = DbContext;
        _applicationOptions = ApplicationOptions;
    }
    public async Task<APIGatewayProxyResponse> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = UserProfile.Create(request.UserName, request.Email, request.Country);
        var user = User.Create(request.CognitoUserId, request.CognitoUserName, request.ConnectionId, userProfile);

        var userWrite = _dbContext.CreateBatchWrite<User>(_applicationOptions.Value.ToOperationConfig());
        userWrite.AddPutItem(user);

        await userWrite.ExecuteAsync(cancellationToken);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new UserProfileDto
            {
                UserId = user.UserId,
                Country = user.Profile.Country,
                Email= user.Profile.Email,
                UserName = user.Profile.UserName
            })
        };
    }
}