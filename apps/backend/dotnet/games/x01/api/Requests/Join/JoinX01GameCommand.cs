namespace Flyingdarts.Backend.Games.X01.Api.Requests.Join;

public class JoinX01GameCommand : Connectable, IRequest<APIGatewayProxyResponse>
{
    public string GameId { get; set; }
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }
    public Game Game { get; set; }

    public Dictionary<string, object> Metadata { get; set; }
}
