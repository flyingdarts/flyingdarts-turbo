using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Tournaments.Start.CQRS
{
    public class StartTournamentCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings X01GameSettings { get; set; }

        internal string ConnectionId { get; set; }
    }
}
