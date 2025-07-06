
namespace Flyingdarts.Persistence;

public class X01State : IGameState<X01State>, IPrimaryKeyItem, ISortKeyItem
{
    [DynamoDBHashKey("PK")]
    public string PrimaryKey { get; set; } = $"X01State";

    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; }

    public Game Game { get; set; }
    public List<GameDart> Darts { get; set; } = new List<GameDart>();
    public List<GamePlayer> Players { get; set; } = new List<GamePlayer>();
    public List<User> Users { get; set; } = new List<User>();

    public X01State()
    {

    }

    public static X01State Create(long gameId)
    {
        return new X01State
        {
            SortKey = $"{gameId}"
        };
    }
}