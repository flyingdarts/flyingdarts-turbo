using Flyingdarts.Meetings.Service.Services.DyteMeetingService.Requests;

namespace Flyingdarts.Meetings.Service.Services;

public interface IMeetingService
{
    Task<Meeting?> CreateAsync(string name, CancellationToken cancellationToken);
    Task<Meeting?> GetByIdAsync(Guid meetingId, CancellationToken cancellationToken);
    Task<Meeting?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Meeting>?> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a participant to a meeting and returns the participant token
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> AddParticipantAsync(JoinMeetingRequest request, CancellationToken cancellationToken);
}
