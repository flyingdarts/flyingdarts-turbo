namespace Flyingdarts.Backend.Signalling.OnDisconnect.CQRS;

public class OnDisconnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; }
}