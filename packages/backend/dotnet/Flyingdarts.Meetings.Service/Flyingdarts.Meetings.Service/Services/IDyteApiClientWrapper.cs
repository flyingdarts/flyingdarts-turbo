using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item.Participants;

namespace Flyingdarts.Meetings.Service.Services;

/// <summary>
/// Wrapper interface for DyteApiClient to enable easier testing.
/// This abstraction allows us to mock the Dyte API calls without dealing with complex Kiota-generated types.
/// </summary>
public interface IDyteApiClientWrapper
{
    /// <summary>
    /// Creates a new meeting with the specified request.
    /// </summary>
    /// <param name="request">The meeting creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created meeting response.</returns>
    Task<MeetingsPostResponse> CreateMeetingAsync(CreateMeetingRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a meeting by its unique identifier.
    /// </summary>
    /// <param name="meetingId">The meeting ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The meeting response.</returns>
    Task<WithMeeting_GetResponse> GetMeetingByIdAsync(string meetingId, CancellationToken cancellationToken);

    /// <summary>
    /// Searches for meetings using the specified search criteria.
    /// </summary>
    /// <param name="searchQuery">The search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The meetings response.</returns>
    Task<MeetingsGetResponse> SearchMeetingsAsync(string searchQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all meetings.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The meetings response.</returns>
    Task<MeetingsGetResponse> GetAllMeetingsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a participant to the specified meeting.
    /// </summary>
    /// <param name="meetingId">The meeting ID.</param>
    /// <param name="request">The participant request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The participant response.</returns>
    Task<ParticipantsPostResponse> AddParticipantAsync(
        string meetingId,
        AddParticipantRequest request,
        CancellationToken cancellationToken
    );
}
