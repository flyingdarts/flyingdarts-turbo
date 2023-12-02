using Flyingdarts.Persistence;

namespace Flyingdarts.Backend.Shared.Caching;

public interface ICachingService<T>
{
    void AddGame(Game game);
    void AddPlayer(GamePlayer player);
    void AddDart(GameDart gameDart);
    void AddUser(User user);
}