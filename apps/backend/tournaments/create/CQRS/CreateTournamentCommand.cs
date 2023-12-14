using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Tournaments.Create.CQRS
{
    public class CreateTournamentCommand : IRequest<APIGatewayProxyResponse>
    {
        public IGameSettings? X01GameSettings { get; set; }0
    }
}
