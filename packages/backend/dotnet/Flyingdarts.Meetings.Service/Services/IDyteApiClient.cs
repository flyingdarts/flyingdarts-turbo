using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item;
using Flyingdarts.Meetings.Service.Generated.Dyte.Meetings.Item.Participants;
using Microsoft.Kiota.Abstractions;

namespace Flyingdarts.Meetings.Service.Services;

public interface IDyteApiClient
{
    IMeetingsRequestBuilder Meetings { get; }
}

public interface IMeetingsRequestBuilder
{
    IWithMeeting_ItemRequestBuilder this[Guid position] { get; }
    Task<MeetingsGetResponse> GetAsMeetingsGetResponseAsync(
        Action<
            RequestConfiguration<MeetingsRequestBuilder.MeetingsRequestBuilderGetQueryParameters>
        >? requestConfiguration = default,
        CancellationToken cancellationToken = default
    );
    Task<MeetingsPostResponse> PostAsMeetingsPostResponseAsync(
        CreateMeetingRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default,
        CancellationToken cancellationToken = default
    );
}

public interface IWithMeeting_ItemRequestBuilder
{
    IParticipantsRequestBuilder Participants { get; }
    Task<WithMeeting_GetResponse> GetAsWithMeeting_GetResponseAsync(
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default,
        CancellationToken cancellationToken = default
    );
}

public interface IParticipantsRequestBuilder
{
    Task<ParticipantsPostResponse> PostAsParticipantsPostResponseAsync(
        AddParticipantRequest body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default,
        CancellationToken cancellationToken = default
    );
}
