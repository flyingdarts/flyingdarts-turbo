using System.Threading;

namespace Flyingdarts.Backend.Tournaments.Api.Requests.Participants.Matches.Update
{
    internal class UpdateTournamentMatchCommandHandler : IRequestHandler<UpdateTournamentMatchCommand, APIGatewayProxyResponse>
    {
        public async Task<APIGatewayProxyResponse> Handle(UpdateTournamentMatchCommand request, CancellationToken cancellationToken)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "A yeet"
            };
        }
    }
}
