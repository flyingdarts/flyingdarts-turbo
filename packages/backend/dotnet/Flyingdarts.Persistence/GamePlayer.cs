namespace Flyingdarts.Persistence;

public class GamePlayer : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } = Constants.GamePlayer;

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; } = string.Empty;

    [DynamoDBLocalSecondaryIndexRangeKey("LSI1")]
    public string LSI1 { get; set; } = string.Empty;

    public string PlayerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long GameId { get; set; }
    public string? MeetingToken { get; set; }

    public GamePlayer() { }

    public static GamePlayer Create(long gameId, string playerId, string meetingToken)
    {
        var now = DateTime.UtcNow;

        return new GamePlayer()
        {
            SortKey = $"{gameId}#{playerId}",
            LSI1 = $"{playerId}#{now}#{gameId}",
            CreatedAt = now,
            PlayerId = playerId,
            GameId = gameId,
            MeetingToken = meetingToken,
        };
    }
}
