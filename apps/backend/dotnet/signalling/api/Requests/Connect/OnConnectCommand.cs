using Amazon.Lambda.APIGatewayEvents;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api.Requests.Connect;

public class OnConnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; } = string.Empty;
    public string AuthProviderUserId { get; set; } = string.Empty;
    public string AuthressToken { get; set; } = string.Empty;
}
