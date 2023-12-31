namespace Flyingdarts.Backend.Tournaments.Api.Requests.Start
{
    public class StartTournamentCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings X01GameSettings { get; set; }

        internal string ConnectionId { get; set; }
    }
}
