namespace Flyingdarts.Persistence;

public class FriendRequest : IPrimaryKeyItem, ISortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } // "FRIEND#REQUEST"

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; } // "{TargetUserId}#{RequesterId}#{CreatedAt.Ticks}"

    public string RequesterId { get; set; }
    public string TargetUserId { get; set; }
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
            Status = FriendRequestStatus.Pending
        };
    }
}
