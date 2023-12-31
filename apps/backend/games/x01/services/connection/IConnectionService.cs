namespace Flyingdarts.Backend.Games.X01.Services.Connection
{
    public interface IConnectionService
    {
        Task UpdateConnectionIdAsync(string playerId, string connectionId, CancellationToken cancellationToken);
    }
}
