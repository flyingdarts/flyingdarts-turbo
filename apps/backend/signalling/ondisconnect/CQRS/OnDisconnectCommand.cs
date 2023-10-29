using Amazon.Lambda.APIGatewayEvents;
using MediatR;

public class OnDisconnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; }
}