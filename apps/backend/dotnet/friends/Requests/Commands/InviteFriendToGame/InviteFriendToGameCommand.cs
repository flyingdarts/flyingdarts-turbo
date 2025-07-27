namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.InviteFriendToGame;

/// <summary>
/// Invite a friend to a game
/// </summary>
public record InviteFriendToGameCommand : IRequest<APIGatewayProxyResponse>
{
    public string InviterId { get; set; } = string.Empty;
    public List<string> FriendIds { get; set; } = new();
    public string GameType { get; set; } = string.Empty;
    public string? GameId { get; set; }
    public string? Message { get; set; }
}
