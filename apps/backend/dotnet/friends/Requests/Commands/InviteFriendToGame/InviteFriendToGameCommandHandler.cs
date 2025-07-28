using System;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;

namespace Flyingdarts.Backend.Friends.Api.Requests.Commands.InviteFriendToGame;

public class InviteFriendToGameCommandHandler : IRequestHandler<InviteFriendToGameCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IOptions<ApplicationOptions> _options;

    public InviteFriendToGameCommandHandler(IDynamoDBContext dbContext, IOptions<ApplicationOptions> options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async Task<APIGatewayProxyResponse> Handle(InviteFriendToGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine(
                $"[InviteFriendToGameCommandHandler] User {request.InviterId} inviting {request.FriendIds.Count} friends to game type: {request.GameType}"
            );

            // Validate request
            if (!request.FriendIds.Any())
            {
                Console.WriteLine($"[InviteFriendToGameCommandHandler] No friends selected for invitation by user: {request.InviterId}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "At least one friend must be selected" }),
                    Headers = GetCorsHeaders(),
                };
            }

            if (string.IsNullOrWhiteSpace(request.GameType))
            {
                Console.WriteLine($"[InviteFriendToGameCommandHandler] GameType is missing for user: {request.InviterId}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "GameType is required" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Verify all friend relationships exist
            var validFriends = new List<string>();
            foreach (var friendId in request.FriendIds)
            {
                var isFriend = await VerifyFriendshipAsync(request.InviterId, friendId, cancellationToken);
                if (isFriend)
                {
                    validFriends.Add(friendId);
                }
                else
                {
                    Console.WriteLine(
                        $"[InviteFriendToGameCommandHandler] User {friendId} is not a friend of {request.InviterId}, skipping invitation"
                    );
                }
            }

            Console.WriteLine(
                $"[InviteFriendToGameCommandHandler] Valid friends for invitation: {validFriends.Count}/{request.FriendIds.Count}"
            );

            if (!validFriends.Any())
            {
                Console.WriteLine($"[InviteFriendToGameCommandHandler] No valid friends found to invite for user: {request.InviterId}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "No valid friends found to invite" }),
                    Headers = GetCorsHeaders(),
                };
            }

            // Send game invitations (this would integrate with your existing game system)
            // For now, we'll create invitation records and notify friends
            await SendGameInvitationsAsync(
                request.InviterId,
                validFriends,
                request.GameType,
                request.GameId,
                request.Message,
                cancellationToken
            );
            Console.WriteLine(
                $"[InviteFriendToGameCommandHandler] Successfully sent {validFriends.Count} game invitations for user: {request.InviterId}"
            );

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(
                    new
                    {
                        message = "Game invitations sent successfully",
                        invitedFriends = validFriends.Count,
                        invalidFriends = request.FriendIds.Count - validFriends.Count,
                    }
                ),
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

    private async Task<bool> VerifyFriendshipAsync(string userId, string friendId, CancellationToken cancellationToken)
    {
        try
        {
            var queryFilter = new QueryFilter("PK", QueryOperator.Equal, Constants.FriendRelationship);
            queryFilter.AddCondition("SK", QueryOperator.Equal, $"{userId}#{friendId}");

            var queryConfig = new QueryOperationConfig { Filter = queryFilter };

            var results = await _dbContext
                .FromQueryAsync<FriendRelationship>(queryConfig, _options.Value.ToOperationConfig())
                .GetRemainingAsync(cancellationToken);

            return results.Any(f => f.Status == FriendshipStatus.Accepted);
        }
        catch
        {
            return false;
        }
    }

    private async Task SendGameInvitationsAsync(
        string inviterId,
        List<string> friendIds,
        string gameType,
        string? gameId,
        string? message,
        CancellationToken cancellationToken
    )
    {
        // Create game invitation records
        var invitations = new List<GameInvitation>();
        var now = DateTime.UtcNow;

        foreach (var friendId in friendIds)
        {
            var invitation = new GameInvitation
            {
                SortKey = $"{friendId}#{inviterId}#{now.Ticks}",
                InviterId = inviterId,
                InviteeId = friendId,
                GameType = gameType,
                GameId = gameId,
                Message = message,
                CreatedAt = now,
                Status = GameInvitationStatus.Pending,
            };
            invitations.Add(invitation);
        }

        // Save invitations
        var batch = _dbContext.CreateBatchWrite<GameInvitation>(_options.Value.ToOperationConfig());
        foreach (var invitation in invitations)
        {
            batch.AddPutItem(invitation);
        }
        await batch.ExecuteAsync(cancellationToken);

        // TODO: Send real-time notifications to friends via WebSocket
        // This would integrate with your existing signalling system
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

// Game invitation model
public class GameInvitation : IPrimaryKeyItem, ISortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } = "GAME#INVITATION";

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; } = string.Empty;

    public string InviterId { get; set; } = string.Empty;
    public string InviteeId { get; set; } = string.Empty;
    public string GameType { get; set; } = string.Empty;
    public string? GameId { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public GameInvitationStatus Status { get; set; }
}

public enum GameInvitationStatus
{
    Pending,
    Accepted,
    Declined,
    Expired,
}
