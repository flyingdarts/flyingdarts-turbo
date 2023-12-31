namespace Flyingdarts.Backend.Tournaments.Api.Requests.Participants.Matches.Update
{
    public class UpdateTournamentMatchCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings X01GameSettings { get; set; }

        internal string ConnectionId { get; set; }
    }
}
