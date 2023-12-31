public class GetUserProfileQuery : IRequest<APIGatewayProxyResponse>
{
    public string CognitoUserName { get; set; }
}