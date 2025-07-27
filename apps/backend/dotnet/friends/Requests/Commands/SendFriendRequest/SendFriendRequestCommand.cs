namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.SendFriendRequest;

public record SendFriendRequestCommand : IRequest<APIGatewayProxyResponse>
{
    public string RequesterId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public string? Message { get; set; }
}
