namespace Flyingdarts.Backend.Friends.Api.Requests.Queries.GetFriends;

public record GetFriendsQuery : IRequest<APIGatewayProxyResponse>
{
    public string UserId { get; set; } = string.Empty;
}
