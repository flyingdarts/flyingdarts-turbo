namespace Flyingdarts.Backend.Signalling.OnDefault.CQRS
{
    public class OnDefaultCommand : IRequest<APIGatewayProxyResponse>
    {
        public string Message { get; set; }
        public Guid Owner { get; set; }
        public string Date { get; set; }
        public string ConnectionId { get; set; }
        internal AmazonApiGatewayManagementApiClient ApiGatewayManagementApiClient { get; set; }
    }
}
