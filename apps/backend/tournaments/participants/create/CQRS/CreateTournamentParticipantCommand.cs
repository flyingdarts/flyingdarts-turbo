using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Tournaments.Participants.Create.CQRS
{
    public class CreateTournamentParticipantCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings X01GameSettings { get; set; }

        internal string ConnectionId { get; set; }
    }
}
