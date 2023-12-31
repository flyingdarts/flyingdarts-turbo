namespace Flyingdarts.Backend.Tournaments.Api.Requests.Participants.Add
{
    public class CreateTournamentParticipantCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings X01GameSettings { get; set; }

        internal string ConnectionId { get; set; }
    }
}
