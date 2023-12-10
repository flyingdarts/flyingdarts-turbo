namespace Flyingdarts.Backend.Games.X01.Create.CQRS;
public class CreateX01GameCommand : IRequest<APIGatewayProxyResponse>
{
    public string PlayerId { get; set; } // so we know who made it
    public int Sets { get; set; }
    public int Legs { get; set; }
    public string GameId { get; set; } // client needs to have this
    internal string ConnectionId { get; set; }
}