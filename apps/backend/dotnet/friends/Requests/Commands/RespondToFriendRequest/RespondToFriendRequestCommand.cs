namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.RespondToFriendRequest;

public record RespondToFriendRequestCommand : IRequest<APIGatewayProxyResponse>
{
    public string RequestId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty; // The user responding to the request
    public bool Accept { get; set; }
}
