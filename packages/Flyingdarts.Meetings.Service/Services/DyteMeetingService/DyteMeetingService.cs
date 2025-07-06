namespace Flyingdarts.Meetings.Service.Services.DyteMeetingService;

public class DyteMeetingService : IMeetingService
{
    const string PresetName = "group_call_participant";
    const RegionEnum PreferredRegion = RegionEnum.EuCentral1;

    private IDyteApiClient _dyteApiClient;

    public DyteMeetingService(IDyteApiClient dyteApiClient)
    {
        _dyteApiClient = dyteApiClient;
    }

    public async Task<Meeting> CreateAsync(string userId, CancellationToken cancellationToken)
    {
        var request = new CreateMeetingRequest
        {
            Title = userId,
            PreferredRegion = PreferredRegion
        };
        var response = await _dyteApiClient
            .Meetings
            .PostAsMeetingsPostResponseAsync(request, cancellationToken: cancellationToken);

        return response.Data;
    }

    public async Task<Meeting?> GetByIdAsync(string meetingId, CancellationToken cancellationToken)
    {
        var meetingGuid = Guid.Parse(meetingId);
        var response = await _dyteApiClient
            .Meetings[meetingGuid]
            .GetAsWithMeeting_GetResponseAsync(cancellationToken: cancellationToken);

        return response.Data;
    }

    public async Task<Meeting?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var response = await _dyteApiClient
            .Meetings
            .GetAsMeetingsGetResponseAsync(
                config => config.QueryParameters.Search = name,
                cancellationToken
            );

        return response.Data.SingleOrDefault();
    }

    public async Task<IEnumerable<Meeting>?> GetAllAsync(CancellationToken cancellationToken)
    {
        var response = await _dyteApiClient
            .Meetings
            .GetAsMeetingsGetResponseAsync(cancellationToken: cancellationToken);

        return response.Data;
    }

    public async Task<Participant?> AddParticipantAsync(
        string meetingId,
        string particpantName,
        CancellationToken cancellationToken
    )
    {
        var meetingGuid = Guid.Parse(meetingId);
        var request = new AddParticipantRequest { Name = particpantName, PresetName = PresetName };
        var response = await _dyteApiClient
            .Meetings[meetingGuid]
            .Participants
            .PostAsParticipantsPostResponseAsync(request, cancellationToken: cancellationToken);

        return response.Data;
    }
}
