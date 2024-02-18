using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyingdarts.Backend.Games.X01.Api.Requests.WebRPC
{
    public record WebRPCVideoCommandHandler(IAmazonApiGatewayManagementApi ApiGatewayClient, ConnectionService ConnectionService, IDynamoDbService DynamoDbService) : IRequestHandler<WebRPCVideoCommand, APIGatewayProxyResponse>
    {
        public async Task<APIGatewayProxyResponse> Handle(WebRPCVideoCommand request, CancellationToken cancellationToken)
        {
            var socketMessage = new SocketMessage<WebRPCVideoCommand>
            {
                Action = "games/x01/webrtc",
                Message = request
            };

            User user = null;

            if (request.Type == "answer")
            {
                user = await DynamoDbService.ReadUserAsync(request.FromUser, cancellationToken);
            } else
            {
                user = await DynamoDbService.ReadUserAsync(request.ToUser, cancellationToken);
            }

            await NotifyRoomAsync(socketMessage, user.ConnectionId, cancellationToken);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(socketMessage)
            };
        }
        private static QueryOperationConfig QueryConfig(string toUserId)
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "FD#USER");
            queryFilter.AddCondition("SK", QueryOperator.BeginsWith, toUserId);
            return new QueryOperationConfig { Filter = queryFilter };
        }

        private async Task NotifyRoomAsync(SocketMessage<WebRPCVideoCommand> request, string connectionId, CancellationToken cancellationToken)
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
