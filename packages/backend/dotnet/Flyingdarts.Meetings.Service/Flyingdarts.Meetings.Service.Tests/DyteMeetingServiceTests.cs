using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Meetings.Service.Services.DyteMeetingService;
using Flyingdarts.Meetings.Service.Services.DyteMeetingService.Requests;

namespace Flyingdarts.Meetings.Service.Tests;

// Test data records for better reusability and immutability
internal record TestMeetingData(Guid Id, string Name)
{
    public string Title => $"{Name} Flyingdarts Room";

    public Meeting ToMeeting() => new() { Id = Id, Title = Title };
}

internal record TestParticipantData(string Name, string Id, string PresetName = "group_call_participant");

internal record TestToken(string Value);

/// <summary>
/// Unit tests for the DyteMeetingService class.
/// Tests various meeting operations including creation, retrieval, and participant management.
/// Uses modern C# features for improved maintainability and reusability.
/// </summary>
public class DyteMeetingServiceTests
{
    private readonly Mock<IDyteApiClientWrapper> mockDyteClient;
    private readonly DyteMeetingService service;

    // Modern constructor initialization
    public DyteMeetingServiceTests()
    {
        mockDyteClient = new Mock<IDyteApiClientWrapper>();

        service = new DyteMeetingService(mockDyteClient.Object);
    }

    // Test data constants using collection expressions and records
    private static readonly TestMeetingData DefaultMeeting = new(Guid.NewGuid(), "TestMeeting");
    private static readonly TestParticipantData DefaultParticipant = new("John Doe", "participant123");
    private static readonly TestToken DefaultToken = new("auth_token_123");

    // Helper methods for common mock setups
    private void SetupCreateMeetingSuccess(TestMeetingData meetingData) =>
        mockDyteClient
            .Setup(x =>
                x.CreateMeetingAsync(It.Is<CreateMeetingRequest>(req => req.Title == meetingData.Title), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new MeetingsPostResponse { Data = meetingData.ToMeeting() });

    private void SetupCreateMeetingFailure() =>
        mockDyteClient
            .Setup(x => x.CreateMeetingAsync(It.IsAny<CreateMeetingRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unknown error from dyte client"));

    private void SetupGetMeetingById(Guid meetingId, Meeting? meeting = null) =>
        mockDyteClient
            .Setup(x => x.GetMeetingByIdAsync(meetingId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WithMeeting_GetResponse { Data = meeting });

    private void SetupSearchMeetings(string searchTitle, IEnumerable<Meeting> meetings) =>
        mockDyteClient
            .Setup(x => x.SearchMeetingsAsync(searchTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MeetingsGetResponse { Data = meetings.ToList() });

    private void SetupGetAllMeetings(IEnumerable<Meeting> meetings) =>
        mockDyteClient
            .Setup(x => x.GetAllMeetingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MeetingsGetResponse { Data = meetings.ToList() });

    private void SetupAddParticipant(Guid meetingId, TestParticipantData participant, string? token = null) =>
        mockDyteClient
            .Setup(x =>
                x.AddParticipantAsync(
                    meetingId.ToString(),
                    It.Is<AddParticipantRequest>(req =>
                        req.Name == participant.Name
                        && req.CustomParticipantId == participant.Id
                        && req.PresetName == participant.PresetName
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ParticipantsPostResponse { Data = token is not null ? new ParticipantsPostResponse_data { Token = token } : null }
            );

    // Generic helper for exception testing
    private async Task<TException> AssertThrowsWithMessage<TException>(
        Func<Task> action,
        string expectedMessage,
        string? expectedParamName = null
    )
        where TException : Exception
    {
        var exception = await Assert.ThrowsAsync<TException>(action);
        Assert.Contains(expectedMessage, exception.Message);

        if (expectedParamName is not null && exception is ArgumentException argException)
        {
            Assert.Equal(expectedParamName, argException.ParamName);
        }

        return exception;
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WhenSuccessful_ReturnsMeetingId()
    {
        // Arrange
        var meetingData = DefaultMeeting;
        SetupCreateMeetingSuccess(meetingData);

        // Act
        var result = await service.CreateAsync(meetingData.Name, CancellationToken.None);

        // Assert
        Assert.Equal(meetingData.Id, result.Id);
        VerifyCreateMeetingCalled(meetingData.Title, Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenExceptionOccurs_ReturnsNull()
    {
        // Arrange
        SetupCreateMeetingFailure();

        // Act
        var result = await service.CreateAsync(DefaultMeeting.Name, CancellationToken.None);

        // Assert
        Assert.Null(result);
        VerifyCreateMeetingCalled(DefaultMeeting.Title, Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WhenNameIsNullOrEmpty_ThrowsArgumentNullException(string? invalidName)
    {
        // Act & Assert
        await AssertThrowsWithMessage<ArgumentNullException>(
            () => service.CreateAsync(invalidName!, CancellationToken.None),
            "Meeting name cannot be null or empty",
            "name"
        );

        // Verify the Dyte client was never called due to validation failure
        mockDyteClient.Verify(x => x.CreateMeetingAsync(It.IsAny<CreateMeetingRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private void VerifyCreateMeetingCalled(string expectedTitle, Func<Times> times) =>
        mockDyteClient.Verify(
            x => x.CreateMeetingAsync(It.Is<CreateMeetingRequest>(req => req.Title == expectedTitle), It.IsAny<CancellationToken>()),
            times
        );

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenMeetingExists_ReturnsMeeting()
    {
        // Arrange
        var meetingData = DefaultMeeting;
        var expectedMeeting = meetingData.ToMeeting();
        SetupGetMeetingById(meetingData.Id, expectedMeeting);

        // Act
        var result = await service.GetByIdAsync(meetingData.Id, CancellationToken.None);

        // Assert
        Assert.Equal(expectedMeeting, result);
        Assert.Equal(meetingData.Id, result?.Id);
        Assert.Equal(meetingData.Title, result?.Title);
        VerifyGetMeetingByIdCalled(meetingData.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMeetingDoesNotExist_ReturnsNull()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        SetupGetMeetingById(meetingId);

        // Act
        var result = await service.GetByIdAsync(meetingId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMeetingIdIsEmpty_ThrowsArgumentException()
    {
        // Act & Assert
        await AssertThrowsWithMessage<ArgumentException>(
            () => service.GetByIdAsync(Guid.Empty, CancellationToken.None),
            "Meeting ID cannot be empty",
            "meetingId"
        );
    }

    private void VerifyGetMeetingByIdCalled(Guid meetingId) =>
        mockDyteClient.Verify(x => x.GetMeetingByIdAsync(meetingId.ToString(), It.IsAny<CancellationToken>()), Times.Once);

    #endregion

    #region GetByNameAsync Tests

    [Fact]
    public async Task GetByNameAsync_WhenMeetingExists_ReturnsMeeting()
    {
        // Arrange
        var meetingData = DefaultMeeting;
        var expectedMeeting = meetingData.ToMeeting();
        SetupSearchMeetings(meetingData.Title, new[] { expectedMeeting });

        // Act
        var result = await service.GetByNameAsync(meetingData.Name, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meetingData.Id, result.Id);
        Assert.Equal(meetingData.Title, result.Title);
    }

    [Fact]
    public async Task GetByNameAsync_WhenMeetingDoesNotExist_ReturnsNull()
    {
        // Arrange
        const string nonExistentName = "NonExistentMeeting";
        SetupSearchMeetings($"{nonExistentName} Flyingdarts Room", new List<Meeting>());

        // Act
        var result = await service.GetByNameAsync(nonExistentName, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetByNameAsync_WhenNameIsNullOrEmpty_ThrowsArgumentNullException(string? invalidName)
    {
        // Act & Assert
        await AssertThrowsWithMessage<ArgumentNullException>(
            () => service.GetByNameAsync(invalidName!, CancellationToken.None),
            "Meeting name cannot be null or empty",
            "name"
        );
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenMeetingsExist_ReturnsMeetings()
    {
        // Arrange
        var expectedMeetings = new[]
        {
            new Meeting { Id = Guid.NewGuid(), Title = "Meeting 1" },
            new Meeting { Id = Guid.NewGuid(), Title = "Meeting 2" },
        };

        SetupGetAllMeetings(expectedMeetings);

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "Meeting 1");
        Assert.Contains(result, m => m.Title == "Meeting 2");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoMeetingsExist_ReturnsEmptyCollection()
    {
        // Arrange
        SetupGetAllMeetings(new List<Meeting>());

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region AddParticipantAsync Tests

    [Fact]
    public async Task AddParticipantAsync_WhenSuccessful_ReturnsToken()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var participant = DefaultParticipant;
        var expectedToken = DefaultToken.Value;

        var request = new JoinMeetingRequest(meetingId, participant.Name, participant.Id, participant.PresetName);
        SetupAddParticipant(meetingId, participant, expectedToken);

        // Act
        var result = await service.AddParticipantAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedToken, result);
        VerifyAddParticipantCalled(meetingId, participant);
    }

    [Fact]
    public async Task AddParticipantAsync_WhenExceptionOccurs_ReturnsNull()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var request = new JoinMeetingRequest(meetingId, DefaultParticipant.Name, DefaultParticipant.Id);

        mockDyteClient
            .Setup(x => x.AddParticipantAsync(It.IsAny<string>(), It.IsAny<AddParticipantRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("API Error"));

        // Act
        var result = await service.AddParticipantAsync(request, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddParticipantAsync_WhenResponseDataIsNull_ReturnsNull()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var request = new JoinMeetingRequest(meetingId, DefaultParticipant.Name, DefaultParticipant.Id);
        SetupAddParticipant(meetingId, DefaultParticipant); // No token provided = null data

        // Act
        var result = await service.AddParticipantAsync(request, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    private void VerifyAddParticipantCalled(Guid meetingId, TestParticipantData participant) =>
        mockDyteClient.Verify(
            x =>
                x.AddParticipantAsync(
                    meetingId.ToString(),
                    It.Is<AddParticipantRequest>(req =>
                        req.Name == participant.Name
                        && req.PresetName == participant.PresetName
                        && req.CustomParticipantId == participant.Id
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

    #endregion
}
