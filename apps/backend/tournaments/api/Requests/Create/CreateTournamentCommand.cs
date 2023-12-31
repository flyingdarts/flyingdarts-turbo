namespace Flyingdarts.Backend.Tournaments.Api.Requests.Create
{
    public class CreateTournamentCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings X01GameSettings { get; set; }

        internal string ConnectionId { get; set; }
    }
}
