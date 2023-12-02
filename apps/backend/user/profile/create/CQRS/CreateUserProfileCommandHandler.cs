using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Flyingdarts.Persistence;
using MediatR;
using Microsoft.Extensions.Options;
using ApplicationOptions = Flyingdarts.Shared.ApplicationOptions;
using Flyingdarts.Backend.Shared.Models;

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

        var socketMessage = new SocketMessage<CreateUserProfileResponse>
        {
            Action = "user/profile/create",
            Message = CreateUserProfileResponse.From(user, userProfile)
        };

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(socketMessage)
        };
    }

    class CreateUserProfileResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

        public static CreateUserProfileResponse From(User user, UserProfile userProfile)
        {
            return new CreateUserProfileResponse
            {
                UserId = user.UserId.ToString(),
                UserName = userProfile.UserName,
                Email = userProfile.Email,
                Country = userProfile.Country
            };
        }
    }
}