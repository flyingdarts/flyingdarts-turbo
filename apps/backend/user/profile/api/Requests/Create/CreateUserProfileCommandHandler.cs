using Flyingdarts.Persistence;
using System;

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
        try
        {
            var userProfile = UserProfile.Create(request.UserName, request.Email, request.Country);
            var user = User.Create(request.AuthProviderUserId, request.ConnectionId, userProfile);

            var userWrite = _dbContext.CreateBatchWrite<User>(_applicationOptions.Value.ToOperationConfig());
            userWrite.AddPutItem(user);

            await userWrite.ExecuteAsync(cancellationToken);

            return new APIGatewayProxyResponse
            {
                StatusCode = 201,
                Body = JsonSerializer.Serialize(new UserProfileDto
                {
                    UserId = user.UserId,
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
}