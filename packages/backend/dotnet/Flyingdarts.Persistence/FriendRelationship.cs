namespace Flyingdarts.Persistence;

public class FriendRelationship : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } // "FRIEND#RELATIONSHIP"

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; } // "{RequesterId}#{FriendId}"

    [DynamoDBLocalSecondaryIndexRangeKey("LSI1")]
    public string LSI1 { get; set; } // "{FriendId}#{RequesterId}" (for reverse lookup)

    public string RequesterId { get; set; }
    public string FriendId { get; set; }
    public FriendshipStatus Status { get; set; } // Pending, Accepted, Blocked
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string? RequestMessage { get; set; }

    public static FriendRelationship Create(string requesterId, string targetUserId, DateTime requestTime, string message)
    {
        var now = DateTime.Now;

        return new FriendRelationship
        {
            PrimaryKey = "FRIEND#RELATIONSHIP",
            SortKey = $"{requesterId}#{targetUserId}",
            LSI1 = $"{targetUserId}#{requesterId}",
            RequesterId = requesterId,
            FriendId = targetUserId,
            Status = FriendshipStatus.Accepted,
            CreatedAt = requestTime,
            AcceptedAt = now,
            RequestMessage = message
        };
    }
}