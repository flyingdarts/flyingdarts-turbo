namespace Flyingdarts.Persistence;

public class FriendRequest : IPrimaryKeyItem, ISortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } = Constants.FriendRequest; // "FRIEND#REQUEST"

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; } = string.Empty; // "{TargetUserId}#{RequesterId}#{CreatedAt.Ticks}"

    public string RequesterId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public FriendRequestStatus Status { get; set; }

    public static FriendRequest Create(string requesterId, string targetUserId, string message)
    {
        var now = DateTime.UtcNow;

        return new FriendRequest
        {
            PrimaryKey = Constants.FriendRequest,
            SortKey = $"{targetUserId}#{requesterId}",
            RequesterId = requesterId,
            TargetUserId = targetUserId,
            Message = message,
            CreatedAt = now,
            Status = FriendRequestStatus.Pending,
        };
    }
}
