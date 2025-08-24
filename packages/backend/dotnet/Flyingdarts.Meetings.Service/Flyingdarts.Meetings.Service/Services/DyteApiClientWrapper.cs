using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item.Participants;

namespace Flyingdarts.Meetings.Service.Services;

/// <summary>
/// Concrete implementation of IDyteApiClientWrapper that wraps the generated DyteApiClient.
/// This wrapper simplifies testing by providing a clean interface that can be easily mocked.
/// </summary>
public class DyteApiClientWrapper : IDyteApiClientWrapper
{
    private readonly DyteApiClient _client;

    /// <summary>
    /// Initializes a new instance of the DyteApiClientWrapper.
    /// </summary>
    /// <param name="client">The Dyte API client.</param>
    public DyteApiClientWrapper(DyteApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <inheritdoc />
    public async Task<MeetingsPostResponse> CreateMeetingAsync(
        CreateMeetingRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await _client.Meetings.PostAsMeetingsPostResponseAsync(
            request,
            cancellationToken: cancellationToken
        );
        return result ?? new MeetingsPostResponse();
    }

    /// <inheritdoc />
    public async Task<WithMeeting_GetResponse> GetMeetingByIdAsync(
        string meetingId,
        CancellationToken cancellationToken
    )
    {
        if (!Guid.TryParse(meetingId, out var meetingGuid))
        {
            throw new ArgumentException("Meeting ID must be a valid GUID", nameof(meetingId));
        }
        var result = await _client
            .Meetings[meetingGuid]
            .GetAsWithMeeting_GetResponseAsync(cancellationToken: cancellationToken);
        return result ?? new WithMeeting_GetResponse();
    }

    /// <inheritdoc />
    public async Task<MeetingsGetResponse> SearchMeetingsAsync(
        string searchQuery,
        CancellationToken cancellationToken
    )
    {
        var result = await _client.Meetings.GetAsMeetingsGetResponseAsync(
            config =>
            {
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    config.QueryParameters.Search = searchQuery;
                }
            },
            cancellationToken
        );
        return result ?? new MeetingsGetResponse();
    }

    /// <inheritdoc />
    public async Task<MeetingsGetResponse> GetAllMeetingsAsync(CancellationToken cancellationToken)
    {
        var result = await _client.Meetings.GetAsMeetingsGetResponseAsync(
            cancellationToken: cancellationToken
        );
        return result ?? new MeetingsGetResponse();
    }

    /// <inheritdoc />
    public async Task<ParticipantsPostResponse> AddParticipantAsync(
        string meetingId,
        AddParticipantRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!Guid.TryParse(meetingId, out var meetingGuid))
        {
            throw new ArgumentException("Meeting ID must be a valid GUID", nameof(meetingId));
        }
        var result = await _client
            .Meetings[meetingGuid]
            .Participants.PostAsParticipantsPostResponseAsync(request, null, cancellationToken);
        return result ?? new ParticipantsPostResponse();
    }
}
