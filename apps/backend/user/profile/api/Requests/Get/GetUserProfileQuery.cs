public class GetUserProfileQuery : IRequest<APIGatewayProxyResponse>
{
    public string AuthProviderUserId { get; set; }
}