using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Persistence;
using MediatR;

public class JoinX01GameCommand : IRequest<APIGatewayProxyResponse>
{
    public string GameId { get; set; }
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }
    public Game Game { get; set; }
    internal List<GamePlayer> Players { get; set; }
    internal List<GameDart> Darts { get; set; }
    internal List<User> Users { get; set; }
    internal string ConnectionId { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
