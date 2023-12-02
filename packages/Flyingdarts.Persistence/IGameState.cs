namespace Flyingdarts.Persistence;

public interface IGameState
{
    public Game Game { get; set; }
    public List<GameDart> Darts { get; set; }
    public List<GamePlayer> Players { get; set; }
    public List<User> Users { get; set; }
}