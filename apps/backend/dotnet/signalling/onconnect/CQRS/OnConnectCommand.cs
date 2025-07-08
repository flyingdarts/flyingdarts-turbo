namespace Flyingdarts.Backend.Signalling.OnConnect.CQRS;

public class OnConnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; }
    
    public string UserId { get; set; }
}