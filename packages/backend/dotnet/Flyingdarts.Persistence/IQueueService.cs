namespace Flyingdarts.Persistence;

public interface IQueueService<TState> {
    Task AddRecord(TState record, CancellationToken cancellationToken);
    Task<List<TState>> GetRecords(CancellationToken cancellationToken);
    Task DeleteRecords(IEnumerable<TState> records, CancellationToken cancellationToken);
}
