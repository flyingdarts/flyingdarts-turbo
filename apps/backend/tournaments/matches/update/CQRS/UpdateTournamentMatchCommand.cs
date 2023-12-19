using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Tournaments.Matches.Update.CQRS
{
    public class UpdateTournamentMatchCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings X01GameSettings { get; set; }

        internal string ConnectionId { get; set; }
    }
}
