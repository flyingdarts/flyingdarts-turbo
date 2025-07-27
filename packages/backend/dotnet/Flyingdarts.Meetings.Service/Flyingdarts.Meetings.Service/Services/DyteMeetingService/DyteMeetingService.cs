using System.Text.Json;
using Flyingdarts.Meetings.Service.Services.DyteMeetingService.Requests;

namespace Flyingdarts.Meetings.Service.Services.DyteMeetingService;

/// <summary>
/// Service implementation for managing Dyte meetings, including creation, retrieval, and participant management.
/// This service acts as a bridge between the application and the Dyte API for video conferencing functionality.
/// </summary>
public class DyteMeetingService : IMeetingService
{
    #region Constants

    /// <summary>
    /// Default preset name for meeting participants with standard group call permissions.
    /// </summary>
    private const string PresetName = "group_call_participant";

    /// <summary>
    /// Preferred region for hosting meetings to optimize performance for European users.
    /// </summary>
    private const RegionEnum PreferredRegion = RegionEnum.EuCentral1;

    /// <summary>
    /// Template for generating meeting names with Flyingdarts branding.
    /// </summary>
    private const string MeetingNameTemplate = "{0} Flyingdarts Room";

    #endregion

    #region Fields

    /// <summary>
    /// Wrapper for interacting with the Dyte API.
    /// </summary>
    private readonly IDyteApiClientWrapper _dyteClient;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DyteMeetingService"/> class.
    /// </summary>
    /// <param name="dyteClient">The Dyte API client wrapper for making requests to the Dyte service.</param>
    /// <exception cref="ArgumentNullException">Thrown when dyteClient is null.</exception>
    public DyteMeetingService(IDyteApiClientWrapper dyteClient)
    {
        _dyteClient = dyteClient ?? throw new ArgumentNullException(nameof(dyteClient));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a new meeting with the specified name.
    /// </summary>
    /// <param name="name">The base name for the meeting (will be formatted with Flyingdarts branding).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created meeting ID.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name is null or empty.</exception>
    public async Task<Meeting?> CreateAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Meeting name cannot be null or empty.");

        var request = new CreateMeetingRequest
        {
            Title = GetMeetingName(name),
            PreferredRegion = PreferredRegion
        };

        try
        {
            var response = await _dyteClient.CreateMeetingAsync(request, cancellationToken);
            return response?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Error creating meeting: {ex.Message}");
            Console.WriteLine($"[Error] Stack trace: {ex.StackTrace}");

            return null;
        }
    }

    /// <summary>
    /// Retrieves a meeting by its formatted name.
    /// </summary>
    /// <param name="name">The base name of the meeting to search for.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the meeting if found, otherwise null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name is null or empty.</exception>
    public async Task<Meeting?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Meeting name cannot be null or empty.");

        Console.WriteLine($"Getting meeting by name: {name}");

        var response = await _dyteClient.SearchMeetingsAsync(
            GetMeetingName(name),
            cancellationToken
        );

        var meeting = response.Data.SingleOrDefault();
        Console.WriteLine(
            meeting != null
                ? $"Meeting found with ID: {meeting.Id}"
                : "No meeting found with the specified name."
        );

        return meeting;
    }

    /// <summary>
    /// Retrieves a meeting by its unique identifier.
    /// </summary>
    /// <param name="meetingId">The unique identifier of the meeting.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the meeting if found, otherwise null.</returns>
    /// <exception cref="ArgumentException">Thrown when meetingId is empty.</exception>
    public async Task<Meeting?> GetByIdAsync(Guid meetingId, CancellationToken cancellationToken)
    {
        if (meetingId == Guid.Empty)
            throw new ArgumentException("Meeting ID cannot be empty.", nameof(meetingId));

        Console.WriteLine($"Getting meeting by ID: {meetingId}");

        var response = await _dyteClient.GetMeetingByIdAsync(
            meetingId.ToString(),
            cancellationToken
        );

        Console.WriteLine(
            response.Data != null
                ? $"Meeting found: {response.Data.Title}"
                : "Meeting not found with the specified ID."
        );

        return response.Data;
    }

    /// <summary>
    /// Retrieves all available meetings.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of meetings.</returns>
    public async Task<IEnumerable<Meeting>?> GetAllAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Retrieving all meetings");

        var response = await _dyteClient.GetAllMeetingsAsync(cancellationToken);

        Console.WriteLine($"Retrieved {response.Data?.Count() ?? 0} meetings");
        return response.Data;
    }

    /// <summary>
    /// Adds a participant to an existing meeting and returns their authentication token.
    /// </summary>
    /// <param name="request">The request containing participant details and meeting information.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the participant's authentication token if successful, otherwise null.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    public async Task<string?> AddParticipantAsync(
        JoinMeetingRequest request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(
            $"[UserRequest] Received request to add participant: {JsonSerializer.Serialize(request)}"
        );
        Console.WriteLine(
            $"[DyteRequest] Converted request to Dyte request: {JsonSerializer.Serialize(request.ToDyteRequest())}"
        );

        try
        {
            var response = await _dyteClient.AddParticipantAsync(
                request.MeetingId.ToString(),
                request.ToDyteRequest(),
                cancellationToken
            );

            Console.WriteLine(
                $"[DyteResponse] Response from Dyte: {JsonSerializer.Serialize(response)}"
            );

            return response?.Data?.Token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Error adding participant: {ex.Message}");
            Console.WriteLine($"[Error] Stack trace: {ex.StackTrace}");

            return null;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Formats a meeting name with the Flyingdarts branding template.
    /// </summary>
    /// <param name="name">The base name for the meeting.</param>
    /// <returns>The formatted meeting name with Flyingdarts branding.</returns>
    private static string GetMeetingName(string name) => string.Format(MeetingNameTemplate, name);

    #endregion
}

static class JoinMeetingRequestExtensions
{
    public static AddParticipantRequest ToDyteRequest(this JoinMeetingRequest request)
    {
        return new AddParticipantRequest
        {
            Name = request.ParticipantName,
            PresetName = request.PresetName,
            CustomParticipantId = request.ParticipantId
        };
    }
}
