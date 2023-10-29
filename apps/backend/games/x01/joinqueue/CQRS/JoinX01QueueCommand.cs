using System;
using Amazon.Lambda.APIGatewayEvents;
using MediatR;

public class JoinX01QueueCommand : IRequest<APIGatewayProxyResponse>
{
    public string PlayerId { get; set; }
    public string GameId { get; set; }
    internal string ConnectionId { get; set; }
}