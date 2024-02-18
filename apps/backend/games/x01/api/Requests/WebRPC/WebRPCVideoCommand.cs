namespace Flyingdarts.Backend.Games.X01.Api.Requests.WebRPC
{
    public class WebRPCVideoCommand : Connectable, IRequest<APIGatewayProxyResponse>
    {
        public string Sdp { get; set; }
        public string Type { get; set; }
        public string ToUser { get; set; }
        public string FromUser { get; set; }
    }
}
