namespace Flyingdarts.Persistence;

public class X01Queue : IGameQueue<X01Queue>, IPrimaryKeyItem, ISortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } = $"X01QueueState";

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; }

    public string PlayerId { get; set; }

    public X01GameSettings? X01 { get; set; }

    public int Average { get; set; }

    public DateTime Joined { get; set; }

    public string ConnectionId { get; set; }

    public X01Queue() { }

    public static X01Queue Create(string playerId, int average, string connectionId, X01GameSettings? x01)
    {
        return new X01Queue
        {
            SortKey = $"{playerId}",
            Average = average,
            ConnectionId = connectionId,
            PlayerId = playerId,
            X01 = x01,
            Joined = DateTime.UtcNow,
        };
    }
}
