namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.RemoveFriend;

public class RemoveFriendCommandHandler : IRequestHandler<RemoveFriendCommand, APIGatewayProxyResponse>
{
    private readonly IFriendsDynamoDbService _friendsDynamoDbService;

    public RemoveFriendCommandHandler(IFriendsDynamoDbService friendsDynamoDbService)
    {
        _friendsDynamoDbService = friendsDynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _friendsDynamoDbService.ReadUserByAuthProviderUserIdAsync(request.UserId, cancellationToken);

            Console.WriteLine($"[RemoveFriendCommandHandler] User {user.UserId} attempting to remove friend {request.FriendId}");

            // Validate request
            if (user.UserId == request.FriendId)
            {
                Console.WriteLine($"[RemoveFriendCommandHandler] Self-removal attempted by user: {user.UserId}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "Cannot remove yourself as a friend" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Find both friendship relationships (bidirectional)
            var relationship1 = await _friendsDynamoDbService.GetFriendRelationshipAsync(user.UserId, request.FriendId, cancellationToken);
            var relationship2 = await _friendsDynamoDbService.GetFriendRelationshipAsync(request.FriendId, user.UserId, cancellationToken);

            if (relationship1 == null && relationship2 == null)
            {
                Console.WriteLine($"[RemoveFriendCommandHandler] No friendship found between {request.UserId} and {request.FriendId}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 404,
                    Body = JsonSerializer.Serialize(new { error = "Friendship not found" }),
                    Headers = GetCorsHeaders(),
                };
            }

            Console.WriteLine(
                $"[RemoveFriendCommandHandler] Found friendship relationships. Relationship1: {relationship1 != null}, Relationship2: {relationship2 != null}"
            );

            // Delete both relationships
            await _friendsDynamoDbService.DeleteFriendRelationshipsAsync(relationship1, relationship2, cancellationToken);
            Console.WriteLine(
                $"[RemoveFriendCommandHandler] Successfully removed friendship between {request.UserId} and {request.FriendId}"
            );

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { message = "Friend removed successfully" }),
                Headers = GetCorsHeaders(),
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { error = ex.Message }),
                Headers = GetCorsHeaders(),
            };
        }
    }

    private static Dictionary<string, string> GetCorsHeaders()
    {
        return new Dictionary<string, string>
        {
            { "Access-Control-Allow-Origin", "*" },
            { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
            { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
        };
    }
}
