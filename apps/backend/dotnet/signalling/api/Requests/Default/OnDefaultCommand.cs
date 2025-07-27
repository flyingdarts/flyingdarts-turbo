using Amazon.ApiGatewayManagementApi;
using Amazon.Lambda.APIGatewayEvents;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api.Requests.Default;

public class OnDefaultCommand : IRequest<APIGatewayProxyResponse>
{
    public string Message { get; set; } = string.Empty;
    public Guid Owner { get; set; }
    public string Date { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    internal AmazonApiGatewayManagementApiClient ApiGatewayManagementApiClient { get; set; } =
        null!;
}
