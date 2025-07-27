namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.RemoveFriend;

/// <summary>
/// Remove a friend
/// </summary>
public record RemoveFriendCommand : IRequest<APIGatewayProxyResponse>
{
    public string UserId { get; set; } = string.Empty;
    public string FriendId { get; set; } = string.Empty;
}
