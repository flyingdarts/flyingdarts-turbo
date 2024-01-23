public class CreateUserProfileCommand : IRequest<APIGatewayProxyResponse>
{
    public string AuthProviderUserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }

    internal string ConnectionId { get; set; }
}


    // Command Query Reponsibility Segregation.