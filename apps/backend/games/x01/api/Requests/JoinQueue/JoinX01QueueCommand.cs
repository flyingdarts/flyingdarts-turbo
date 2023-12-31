public class JoinX01QueueCommand : Connectable, IRequest<APIGatewayProxyResponse>
{
    public string PlayerId { get; set; }
    public int Sets { get; set; }
    public int Legs { get; set; }
    public string GameId { get; set; }
}