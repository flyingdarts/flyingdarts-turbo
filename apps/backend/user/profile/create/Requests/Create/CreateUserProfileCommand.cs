using Amazon.Lambda.APIGatewayEvents;
using MediatR;

public class CreateUserProfileCommand : IRequest<APIGatewayProxyResponse>
{
    public string CognitoUserId { get; set; }
    public string CognitoUserName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }

    internal string ConnectionId { get; set; }
}


    // Command Query Reponsibility Segregation.