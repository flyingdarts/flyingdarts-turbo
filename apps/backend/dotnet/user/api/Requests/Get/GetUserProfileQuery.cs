namespace Flyingdarts.Backend.User.Profile.Api.Requests.Get;

public class GetUserProfileQuery : IRequest<APIGatewayProxyResponse>
{
    public string AuthProviderUserId { get; set; }
}