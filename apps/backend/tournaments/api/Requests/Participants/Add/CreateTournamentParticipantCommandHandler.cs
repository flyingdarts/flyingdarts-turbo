using System.Threading;

namespace Flyingdarts.Backend.Tournaments.Api.Requests.Participants.Add
{
    internal class CreateTournamentParticipantCommandHandler : IRequestHandler<CreateTournamentParticipantCommand, APIGatewayProxyResponse>
    {
        public async Task<APIGatewayProxyResponse> Handle(CreateTournamentParticipantCommand request, CancellationToken cancellationToken)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "A yeet"
            };
        }
    }
}
