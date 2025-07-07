using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Meetings.Service.Services.DyteMeetingService;

namespace Flyingdarts.Meetings.Service.Tests;

public class DyteMeetingServiceTests
{
    private readonly Mock<IDyteApiClient> _mockDyteApiClient;
    private readonly Mock<IMeetingsRequestBuilder> _mockMeetingsRequestBuilder;
    private readonly Mock<IWithMeeting_ItemRequestBuilder> _mockMeetingItemRequestBuilder;
    private readonly Mock<IParticipantsRequestBuilder> _mockParticipantsRequestBuilder;
    private readonly DyteMeetingService _service;

    public DyteMeetingServiceTests()
    {
        _mockDyteApiClient = new Mock<IDyteApiClient>();
        _mockMeetingsRequestBuilder = new Mock<IMeetingsRequestBuilder>();
        _mockMeetingItemRequestBuilder = new Mock<IWithMeeting_ItemRequestBuilder>();
        _mockParticipantsRequestBuilder = new Mock<IParticipantsRequestBuilder>();

        _mockDyteApiClient.Setup(x => x.Meetings).Returns(_mockMeetingsRequestBuilder.Object);
        _service = new DyteMeetingService(_mockDyteApiClient.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenSuccessful_ReturnsMeeting()
    {
        // Arrange
        var userId = DateTime.Now.Ticks.ToString();
        var meetingId = Guid.Parse("463143e7-677f-4ec5-add4-1300f3f82b97");
        var expectedMeeting = new Meeting { Id = meetingId, Title = userId };
        var expectedResponse = new MeetingsPostResponse { Data = expectedMeeting };

        _mockMeetingsRequestBuilder
            .Setup(
                x =>
                    x.PostAsMeetingsPostResponseAsync(
                        It.IsAny<CreateMeetingRequest>(),
                        It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.CreateAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedMeeting, result);
        _mockMeetingsRequestBuilder.Verify(
            x =>
                x.PostAsMeetingsPostResponseAsync(
                    It.Is<CreateMeetingRequest>(req => req.Title == userId),
                    It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenMeetingExists_ReturnsMeeting()
    {
        // Arrange
        var meetingId = Guid.Parse("463143e7-677f-4ec5-add4-1300f3f82b97");
        var expectedMeeting = new Meeting { Id = meetingId, Title = "Test Meeting" };
        var expectedResponse = new WithMeeting_GetResponse { Data = expectedMeeting };

        _mockMeetingItemRequestBuilder
            .Setup(
                x =>
                    x.GetAsWithMeeting_GetResponseAsync(
                        It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        // Mock the indexer to return our mock
        _mockMeetingsRequestBuilder
            .Setup(x => x[meetingId])
            .Returns(_mockMeetingItemRequestBuilder.Object);

        // Act
        var result = await _service.GetByIdAsync(meetingId.ToString(), CancellationToken.None);

        // Assert
        Assert.Equal(expectedMeeting, result);
        _mockMeetingItemRequestBuilder.Verify(
            x =>
                x.GetAsWithMeeting_GetResponseAsync(
                    It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenMeetingDoesNotExist_ReturnsNull()
    {
        // Arrange
        var meetingId = Guid.Parse("463143e7-677f-4ec5-add4-1300f3f82b97");
        var expectedResponse = new WithMeeting_GetResponse { Data = null };

        _mockMeetingItemRequestBuilder
            .Setup(
                x =>
                    x.GetAsWithMeeting_GetResponseAsync(
                        It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        _mockMeetingsRequestBuilder
            .Setup(x => x[meetingId])
            .Returns(_mockMeetingItemRequestBuilder.Object);

        // Act
        var result = await _service.GetByIdAsync(meetingId.ToString(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenMeetingExists_ReturnsMeeting()
    {
        // Arrange
        var meetingName = "Test Meeting";
        var expectedMeeting = new Meeting
        {
            Id = Guid.Parse("463143e7-677f-4ec5-add4-1300f3f82b97"),
            Title = meetingName
        };
        var expectedResponse = new MeetingsGetResponse
        {
            Data = new List<Meeting> { expectedMeeting }
        };

        _mockMeetingsRequestBuilder
            .Setup(
                x =>
                    x.GetAsMeetingsGetResponseAsync(
                        It.IsAny<
                            Action<
                                RequestConfiguration<MeetingsRequestBuilder.MeetingsRequestBuilderGetQueryParameters>
                            >
                        >(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetByNameAsync(meetingName, CancellationToken.None);

        // Assert
        Assert.Equal(expectedMeeting, result);
        _mockMeetingsRequestBuilder.Verify(
            x =>
                x.GetAsMeetingsGetResponseAsync(
                    It.IsAny<
                        Action<
                            RequestConfiguration<MeetingsRequestBuilder.MeetingsRequestBuilderGetQueryParameters>
                        >
                    >(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByNameAsync_WhenMeetingDoesNotExist_ReturnsNull()
    {
        // Arrange
        var meetingName = "Non-existent Meeting";
        var expectedResponse = new MeetingsGetResponse { Data = new List<Meeting>() };

        _mockMeetingsRequestBuilder
            .Setup(
                x =>
                    x.GetAsMeetingsGetResponseAsync(
                        It.IsAny<
                            Action<
                                RequestConfiguration<MeetingsRequestBuilder.MeetingsRequestBuilderGetQueryParameters>
                            >
                        >(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetByNameAsync(meetingName, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenMeetingsExist_ReturnsMeetings()
    {
        // Arrange
        var expectedMeetings = new List<Meeting>
        {
            new Meeting
            {
                Id = Guid.Parse("55b4b9c3-2a9f-4040-a1de-35a4cc070498"),
                Title = "Meeting 1"
            },
            new Meeting
            {
                Id = Guid.Parse("482161a5-4c50-44d3-9aa2-345ccfc7f327"),
                Title = "Meeting 2"
            }
        };
        var expectedResponse = new MeetingsGetResponse { Data = expectedMeetings };

        _mockMeetingsRequestBuilder
            .Setup(
                x =>
                    x.GetAsMeetingsGetResponseAsync(
                        It.IsAny<
                            Action<
                                RequestConfiguration<MeetingsRequestBuilder.MeetingsRequestBuilderGetQueryParameters>
                            >
                        >(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedMeetings, result);
        _mockMeetingsRequestBuilder.Verify(
            x =>
                x.GetAsMeetingsGetResponseAsync(
                    It.IsAny<
                        Action<
                            RequestConfiguration<MeetingsRequestBuilder.MeetingsRequestBuilderGetQueryParameters>
                        >
                    >(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddParticipantAsync_WhenSuccessful_ReturnsParticipant()
    {
        // Arrange
        var meetingId = Guid.Parse("463143e7-677f-4ec5-add4-1300f3f82b97");
        var participantId = DateTime.Now.Ticks;
        var participantName = "John Doe";
        var expectedParticipant = new ParticipantsPostResponse_data
        {
            Id = Guid.Parse("65f8e556-3462-45c9-a534-152b14c63013"),
            Name = participantName
        };

        var expectedResponse = new ParticipantsPostResponse { Data = expectedParticipant };

        _mockParticipantsRequestBuilder
            .Setup(
                x =>
                    x.PostAsParticipantsPostResponseAsync(
                        It.IsAny<AddParticipantRequest>(),
                        It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        _mockMeetingItemRequestBuilder
            .Setup(x => x.Participants)
            .Returns(_mockParticipantsRequestBuilder.Object);

        _mockMeetingsRequestBuilder
            .Setup(x => x[meetingId])
            .Returns(_mockMeetingItemRequestBuilder.Object);

        // Act
        var result = await _service.AddParticipantAsync(
            meetingId.ToString(),
            participantName,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(expectedParticipant, result);
        _mockParticipantsRequestBuilder.Verify(
            x =>
                x.PostAsParticipantsPostResponseAsync(
                    It.Is<AddParticipantRequest>(
                        req =>
                            req.Name == participantName
                            && req.PresetName == "group_call_participant"
                    ),
                    It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddParticipantAsync_WhenParticipantCreationFails_ReturnsNull()
    {
        // Arrange
        var meetingId = Guid.Parse("463143e7-677f-4ec5-add4-1300f3f82b97");
        var participantName = "John Doe";
        var expectedResponse = new ParticipantsPostResponse { Data = null };

        _mockParticipantsRequestBuilder
            .Setup(
                x =>
                    x.PostAsParticipantsPostResponseAsync(
                        It.IsAny<AddParticipantRequest>(),
                        It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(expectedResponse);

        _mockMeetingItemRequestBuilder
            .Setup(x => x.Participants)
            .Returns(_mockParticipantsRequestBuilder.Object);

        _mockMeetingsRequestBuilder
            .Setup(x => x[meetingId])
            .Returns(_mockMeetingItemRequestBuilder.Object);

        // Act
        var result = await _service.AddParticipantAsync(
            meetingId.ToString(),
            participantName,
            CancellationToken.None
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_VerifiesRequestContent()
    {
        // Arrange
        var userId = Guid.Parse("6a104ba7-ef61-4cd6-88df-0864c100bedf");
        var meetingId = Guid.Parse("3c9e8423-ee99-4e8e-af39-33de20a13833");
        var playerName = "Mike Pattyn";

        var expectedMeeting = new Meeting { Id = meetingId, Title = playerName };
        var expectedResponse = new MeetingsPostResponse { Data = expectedMeeting };

        CreateMeetingRequest? capturedRequest = null;
        _mockMeetingsRequestBuilder
            .Setup(
                x =>
                    x.PostAsMeetingsPostResponseAsync(
                        It.IsAny<CreateMeetingRequest>(),
                        It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .Callback<
                CreateMeetingRequest,
                Action<RequestConfiguration<DefaultQueryParameters>>,
                CancellationToken
            >((req, config, ct) => capturedRequest = req)
            .ReturnsAsync(expectedResponse);

        // Act
        await _service.CreateAsync(userId.ToString(), CancellationToken.None);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(userId.ToString(), capturedRequest.Title);
        Assert.Equal(
            CreateMeetingRequest_preferred_region.EuCentral1,
            capturedRequest.PreferredRegion
        );
    }
}
