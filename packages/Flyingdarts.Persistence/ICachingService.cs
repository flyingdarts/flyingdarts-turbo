namespace Flyingdarts.Persistence;

public interface ICachingService<T>
{
    void AddGame(Game game);
    void AddPlayer(GamePlayer player);
    void AddDart(GameDart gameDart);
    void AddUser(User user);
}