namespace Flyingdarts.Persistence;

public class Game : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } = Constants.Game;

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; } = string.Empty;

    [DynamoDBLocalSecondaryIndexRangeKey("LSI1")]
    public string LSI1 { get; set; } = string.Empty;

    public long GameId { get; set; }
    public GameType Type { get; set; }
    public GameStatus Status { get; set; }
    public int PlayerCount { get; set; }

    public X01GameSettings X01 { get; set; } = new X01GameSettings(1, 1, false, true, 501);

    public DateTime CreationDate => new(GameId);
    public Guid MeetingIdentifier { get; set; }

    public Game()
    {
        PrimaryKey = Constants.Game;
        GameId = DateTime.UtcNow.Ticks;
        Status = GameStatus.Qualifying;
        SortKey = $"{GameId}#{Status}";
        LSI1 = $"{Status}#{GameId}";
    }

    public static Game Create(int playerCount, X01GameSettings settings, Guid meetingIdentifier)
    {
        return new Game()
        {
            Type = GameType.X01,
            PlayerCount = playerCount,
            X01 = settings,
            MeetingIdentifier = meetingIdentifier,
        };
    }
}
