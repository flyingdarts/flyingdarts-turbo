using Amazon.Lambda.APIGatewayEvents;
using MediatR;

public class UpdateUserProfileCommand : IRequest<APIGatewayProxyResponse>
{
    public string UserId { get;set;}
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    internal string ConnectionId { get; set; }
}