using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item.Participants;
using Microsoft.Kiota.Abstractions;

namespace Flyingdarts.Meetings.Service.Services;

public class DyteApiClientWrapper : IDyteApiClient
{
    private readonly DyteApiClient _dyteApiClient;

    public DyteApiClientWrapper(DyteApiClient dyteApiClient)
    {
        _dyteApiClient = dyteApiClient;
    }

    public IMeetingsRequestBuilder Meetings =>
        new MeetingsRequestBuilderWrapper(_dyteApiClient.Meetings);
}

public class MeetingsRequestBuilderWrapper : IMeetingsRequestBuilder
{
    private readonly MeetingsRequestBuilder _meetingsRequestBuilder;

    public MeetingsRequestBuilderWrapper(MeetingsRequestBuilder meetingsRequestBuilder)
    {
        _meetingsRequestBuilder = meetingsRequestBuilder;
    }

    public IWithMeeting_ItemRequestBuilder this[Guid position] =>
        new WithMeeting_ItemRequestBuilderWrapper(_meetingsRequestBuilder[position]);

    public async Task<MeetingsGetResponse> GetAsMeetingsGetResponseAsync(
        Action<
            RequestConfiguration<MeetingsRequestBuilder.MeetingsRequestBuilderGetQueryParameters>
        >? requestConfiguration = default,
        CancellationToken cancellationToken = default
    )
    {
        return await _meetingsRequestBuilder.GetAsMeetingsGetResponseAsync(
            requestConfiguration,
            cancellationToken
        );
    }

    public async Task<MeetingsPostResponse> PostAsMeetingsPostResponseAsync(
        CreateMeetingRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default,
        CancellationToken cancellationToken = default
    )
    {
        return await _meetingsRequestBuilder.PostAsMeetingsPostResponseAsync(
            body,
            requestConfiguration,
            cancellationToken
        );
    }
}

public class WithMeeting_ItemRequestBuilderWrapper : IWithMeeting_ItemRequestBuilder
{
    private readonly WithMeeting_ItemRequestBuilder _withMeetingItemRequestBuilder;

    public WithMeeting_ItemRequestBuilderWrapper(
        WithMeeting_ItemRequestBuilder withMeetingItemRequestBuilder
    )
    {
        _withMeetingItemRequestBuilder = withMeetingItemRequestBuilder;
    }

    public IParticipantsRequestBuilder Participants =>
        new ParticipantsRequestBuilderWrapper(_withMeetingItemRequestBuilder.Participants);

    public async Task<WithMeeting_GetResponse> GetAsWithMeeting_GetResponseAsync(
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default,
        CancellationToken cancellationToken = default
    )
    {
        return await _withMeetingItemRequestBuilder.GetAsWithMeeting_GetResponseAsync(
            requestConfiguration,
            cancellationToken
        );
    }
}

public class ParticipantsRequestBuilderWrapper : IParticipantsRequestBuilder
{
    private readonly ParticipantsRequestBuilder _participantsRequestBuilder;

    public ParticipantsRequestBuilderWrapper(ParticipantsRequestBuilder participantsRequestBuilder)
    {
        _participantsRequestBuilder = participantsRequestBuilder;
    }

    public async Task<ParticipantsPostResponse> PostAsParticipantsPostResponseAsync(
        AddParticipantRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default,
        CancellationToken cancellationToken = default
    )
    {
        return await _participantsRequestBuilder.PostAsParticipantsPostResponseAsync(
            body,
            requestConfiguration,
            cancellationToken
        );
    }
}
