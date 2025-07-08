namespace Flyingdarts.Backend.Games.X01.Api.Requests.Score;

public class CreateX01ScoreCommand : Connectable, IRequest<APIGatewayProxyResponse>
{
    public string GameId { get; set; }
    public string PlayerId { get; set; }
    public int Score { get; set; }
    public int Input { get; set; }

    internal string ConnectionId { get; set; }
    internal Game Game { get; set; }
}
