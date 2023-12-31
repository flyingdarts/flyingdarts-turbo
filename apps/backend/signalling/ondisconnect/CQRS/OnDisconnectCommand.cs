public class OnDisconnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; }
}