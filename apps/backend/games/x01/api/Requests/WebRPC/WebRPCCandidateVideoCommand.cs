namespace Flyingdarts.Backend.Games.X01.Api.Requests.WebRPC
{
    public class WebRPCCandidateVideoCommand : Connectable, IRequest<APIGatewayProxyResponse>
    {
        public string ToUser { get; set; }
        public string Candidate { get; set; }
    }
}
