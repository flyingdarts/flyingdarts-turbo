using Amazon.Lambda.APIGatewayEvents;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api.Requests.Connect;

/// <summary>
/// This command is sent when a client connects to the signalling server.
/// It is used to authenticate the client and get the client's connection ID.
/// </summary>
public class OnConnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; } = string.Empty;
    public string AuthProviderUserId { get; set; } = string.Empty;
    public string AuthressToken { get; set; } = string.Empty;
}
