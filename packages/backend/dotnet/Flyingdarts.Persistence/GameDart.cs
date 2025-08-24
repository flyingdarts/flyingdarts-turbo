namespace Flyingdarts.Persistence;

public class GameDart : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } = Constants.GameDart;

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; } = string.Empty;

    [DynamoDBLocalSecondaryIndexRangeKey("LSI1")]
    public string LSI1 { get; set; } = string.Empty;

    public Guid Id { get; set; }
    public long GameId { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public int Score { get; set; }
    public int GameScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Leg { get; set; }
    public int Set { get; set; }

    public GameDart() { }

    public static GameDart Create(
        long gameId,
        string playerId,
        int score,
        int gameScore,
        int set,
        int leg
    )
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        return new GameDart()
        {
            Id = id,
            GameId = gameId,
            PlayerId = playerId,
            GameScore = gameScore,
            Score = score,
            Set = set,
            Leg = leg,
            CreatedAt = createdAt,
            SortKey = $"{gameId}#{id}#{playerId}",
            LSI1 = $"{playerId}#{createdAt}",
        };
    }
}
