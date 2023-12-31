using System.Threading;

namespace Flyingdarts.Backend.Tournaments.Api.Requests.Start
{
    internal class StartTournamentCommandHandler : IRequestHandler<StartTournamentCommand, APIGatewayProxyResponse>
    {
        public async Task<APIGatewayProxyResponse> Handle(StartTournamentCommand request, CancellationToken cancellationToken)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "A yeet"
            };
        }
    }
}
