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
    public async Task<MeetingsPostResponse> CreateMeetingAsync(CreateMeetingRequest request, CancellationToken cancellationToken)
    {
        return await _client.Meetings.PostAsMeetingsPostResponseAsync(request, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WithMeeting_GetResponse> GetMeetingByIdAsync(string meetingId, CancellationToken cancellationToken)
    {
        return await _client.Meetings[meetingId].GetAsWithMeeting_GetResponseAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MeetingsGetResponse> SearchMeetingsAsync(string searchQuery, CancellationToken cancellationToken)
    {
        return await _client.Meetings.GetAsMeetingsGetResponseAsync(
            config => config.QueryParameters.Search = searchQuery,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<MeetingsGetResponse> GetAllMeetingsAsync(CancellationToken cancellationToken)
    {
        return await _client.Meetings.GetAsMeetingsGetResponseAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ParticipantsPostResponse> AddParticipantAsync(
        string meetingId,
        AddParticipantRequest request,
        CancellationToken cancellationToken
    )
    {
        return await _client.Meetings[meetingId].Participants.PostAsParticipantsPostResponseAsync(request, null, cancellationToken);
    }
}
