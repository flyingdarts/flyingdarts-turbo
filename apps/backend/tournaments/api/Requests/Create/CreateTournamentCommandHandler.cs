using System.Threading;

namespace Flyingdarts.Backend.Tournaments.Api.Requests.Create
{
    internal class CreateTournamentCommandHandler : IRequestHandler<CreateTournamentCommand, APIGatewayProxyResponse>
    {
        public async Task<APIGatewayProxyResponse> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "A yeet"
            };
        }
    }
}
