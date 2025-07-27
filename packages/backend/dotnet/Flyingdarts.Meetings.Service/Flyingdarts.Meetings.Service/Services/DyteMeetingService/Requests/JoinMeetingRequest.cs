namespace Flyingdarts.Meetings.Service.Services.DyteMeetingService.Requests;

/// <summary>
/// Represents a request to join a participant to an existing meeting.
/// This record encapsulates all the necessary information required to add a participant to a Dyte meeting.
/// </summary>
/// <param name="MeetingId">The unique identifier of the meeting the participant wants to join.</param>
/// <param name="ParticipantName">The display name of the participant that will be shown in the meeting.</param>
/// <param name="ParticipantId">The unique identifier for the participant in the system.</param>
/// <param name="PresetName">
/// The preset configuration that determines the participant's permissions and capabilities in the meeting.
/// Defaults to "group_call_participant" which provides standard video call permissions.
/// </param>
public record JoinMeetingRequest(
    Guid MeetingId,
    string ParticipantName,
    string ParticipantId,
    string PresetName = "group_call_participant"
);
