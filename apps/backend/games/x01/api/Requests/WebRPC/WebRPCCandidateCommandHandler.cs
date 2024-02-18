using Amazon.DynamoDBv2.DocumentModel;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.WebRPC
{
    public record WebRPCCandidateCommandHandler(IAmazonApiGatewayManagementApi ApiGatewayClient, ConnectionService ConnectionService, IDynamoDbService DynamoDbService) : IRequestHandler<WebRPCCandidateVideoCommand, APIGatewayProxyResponse>
    {
        public async Task<APIGatewayProxyResponse> Handle(WebRPCCandidateVideoCommand request, CancellationToken cancellationToken)
        {
            var socketMessage = new SocketMessage<WebRPCCandidateVideoCommand>
            {
                Action = "games/x01/webrtc/candidate",
                Message = request
            };

            var user = await DynamoDbService.ReadUserAsync(request.ToUser, cancellationToken);
            
            await NotifyRoomAsync(socketMessage, user.ConnectionId, cancellationToken);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(socketMessage, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            };
        }

        private async Task NotifyRoomAsync(SocketMessage<WebRPCCandidateVideoCommand> request, string connectionId, CancellationToken cancellationToken)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)));
            if (!string.IsNullOrEmpty(connectionId))
            {
                var postConnectionRequest = new PostToConnectionRequest
                {
                    ConnectionId = connectionId,
                    Data = stream
                };

                stream.Position = 0;

                await ApiGatewayClient.PostToConnectionAsync(postConnectionRequest, cancellationToken);
            }
        }
    }
}
