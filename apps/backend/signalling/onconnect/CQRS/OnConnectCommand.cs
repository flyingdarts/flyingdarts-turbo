﻿public class OnConnectCommand : IRequest<APIGatewayProxyResponse>
{
    public string ConnectionId { get; set; } = null;
}