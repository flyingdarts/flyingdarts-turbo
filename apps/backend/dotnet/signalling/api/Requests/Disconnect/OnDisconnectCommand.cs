using Amazon.Lambda.APIGatewayEvents;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api.Requests.Disconnect;

public class OnDisconnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; } = string.Empty;
}
