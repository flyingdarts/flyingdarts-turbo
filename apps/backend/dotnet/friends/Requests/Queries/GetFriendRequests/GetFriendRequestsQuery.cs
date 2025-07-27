namespace Flyingdarts.Backend.Friends.Api.Requests.Queries.GetFriendRequests;

public record GetFriendRequestsQuery : IRequest<APIGatewayProxyResponse>
{
    public string UserId { get; set; } = string.Empty;
}
