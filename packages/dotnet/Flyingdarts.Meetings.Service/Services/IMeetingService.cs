namespace Flyingdarts.Meetings.Service.Services;

public interface IMeetingService
{
    Task<Meeting?> CreateAsync(string meetingId, CancellationToken cancellationToken);
    Task<Meeting?> GetByIdAsync(string meetingId, CancellationToken cancellationToken);
    Task<Meeting?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Meeting>?> GetAllAsync(CancellationToken cancellationToken);
    Task<Participant?> AddParticipantAsync(
        string meetingId,
        string particpantName,
        CancellationToken cancellationToken
    );
}
