namespace Flyingdarts.Persistence;
[DynamoDBTable("Flyingdarts-Application-Table")]
public class GamePlayer : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; }

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; }

    [DynamoDBLocalSecondaryIndexRangeKey("LSI1")]
    public string LSI1 { get; set; }

    public string PlayerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long GameId { get; set; }

    public GamePlayer()
    {
        PrimaryKey = Constants.GamePlayer;
    }
    public static GamePlayer Create(long gameId, string playerId)
    {
        var now = DateTime.UtcNow;

        return new GamePlayer()
        {
            SortKey = $"{gameId}#{playerId}",
            LSI1 = $"{playerId}#{now}#{gameId}",
            CreatedAt = now,
            PlayerId = playerId,
            GameId = gameId
        };
    }
}