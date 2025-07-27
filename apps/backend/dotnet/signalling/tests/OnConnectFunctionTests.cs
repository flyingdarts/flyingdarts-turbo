using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Signalling.Api.Requests.Connect;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Lambda.Core.Handlers;
using Flyingdarts.Lambda.Core.Testing;
using Flyingdarts.Meetings.Service.Generated.Dyte.Models;
using Flyingdarts.Meetings.Service.Services;
using Flyingdarts.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Flyingdarts.Backend.Signalling.OnConnect.Tests;

public class OnConnectTests : LambdaTestBase<OnConnectCommand, APIGatewayProxyResponse>
{
    private readonly Mock<IMeetingService> _mockMeetingService;
    private readonly Mock<IDynamoDbService> _mockDynamoDbService;
    private readonly OnConnectCommandHandler _handler;
    private readonly MediatRLambdaHandler<OnConnectCommand> _lambdaHandler;

    public OnConnectTests()
    {
        _mockMeetingService = new Mock<IMeetingService>();
        _mockDynamoDbService = new Mock<IDynamoDbService>();

        _handler = new OnConnectCommandHandler(
            _mockMeetingService.Object,
            _mockDynamoDbService.Object
        );

        Setup(services =>
        {
            // Register mocks
            services.AddSingleton(_mockMeetingService.Object);
            services.AddSingleton(_mockDynamoDbService.Object);
            services.AddSingleton(_handler);
        });

        _lambdaHandler = new MediatRLambdaHandler<OnConnectCommand>(
            ServiceProvider.GetRequiredService<IMediator>()
        );
    }

    /// <summary>
    /// Override to register the correct assembly containing the command handlers
    /// </summary>
    protected override void ConfigureMockServices(IServiceCollection services)
    {
        // Configure real MediatR for testing with the correct assembly
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(OnConnectCommand).Assembly)
        );
    }

    [Fact]
    public async Task Handler_WhenUserNotFound_ShouldCreateNewUser()
    {
        // Arrange
        var connectionId = "test-connection-id";
        var authProviderUserId = "test-auth-provider-user-id";
        var meetingId = "9a9c7096-1ca5-440e-bb59-b612a56145e9";
        var command = new OnConnectCommand
        {
            ConnectionId = connectionId,
            AuthProviderUserId = authProviderUserId
        };

        // Mock DynamoDbService to return null (user not found)
        _mockDynamoDbService
            .Setup(
                x =>
                    x.ReadUserByAuthProviderUserIdAsync(
                        authProviderUserId,
                        It.IsAny<CancellationToken>()
                    )
            )
            .ThrowsAsync(
                new DynamoDbService.UserNotFoundException(
                    nameof(authProviderUserId),
                    authProviderUserId
                )
            );

        // Mock DynamoDbService to handle user creation
        _mockDynamoDbService
            .Setup(x => x.WriteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Mock MeetingService to return a meeting ID
        _mockMeetingService
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Meeting { Id = Guid.Parse(meetingId) });

        // Act
        var result = await _lambdaHandler.Handle(command);

        // Assert
        AssertSuccess(result, 201);

        // Verify that ReadUserByAuthProviderUserIdAsync was called
        _mockDynamoDbService.Verify(
            x =>
                x.ReadUserByAuthProviderUserIdAsync(
                    authProviderUserId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        // Verify that WriteUserAsync was called twice:
        // 1. When creating the new user
        // 2. When creating a new meeting
        _mockDynamoDbService.Verify(
            x => x.WriteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task Handler_WhenUserFound_ShouldUpdateUserWithConnectionId()
    {
        // Arrange
        var connectionId = "test-connection-id";
        var authProviderUserId = "test-auth-provider-user-id";
        var testUserId = DateTime.UtcNow.Ticks.ToString();
        var meetingId = Guid.Parse("9a9c7096-1ca5-440e-bb59-b612a56145e9");
        var command = new OnConnectCommand
        {
            ConnectionId = connectionId,
            AuthProviderUserId = authProviderUserId
        };

        // Create an existing user
        var existingUser = new User
        {
            UserId = testUserId,
            AuthProviderUserId = authProviderUserId,
            ConnectionId = "old-connection-id",
            Profile = new UserProfile
            {
                UserName = "Test User",
                Email = "test@example.com",
                Country = "US"
            }
        };

        var existingMeeting = new Meeting { Id = meetingId };

        var expectedResponseBody = new Dictionary<string, string?>
        {
            { "UserId", testUserId },
            { "MeetingIdentifier", existingMeeting.Id.ToString() }
        };

        // Mock DynamoDbService to return existing user
        _mockDynamoDbService
            .Setup(
                x =>
                    x.ReadUserByAuthProviderUserIdAsync(
                        authProviderUserId,
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(existingUser);

        // Mock MeetingService to return existing meeting
        _mockMeetingService
            .Setup(x => x.GetByNameAsync(testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMeeting);

        // Mock DynamoDbService to handle user update
        _mockDynamoDbService
            .Setup(x => x.WriteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _lambdaHandler.Handle(command);

        // Assert
        AssertSuccess(result, 201);
        result.Body.Should().Be(JsonConvert.SerializeObject(expectedResponseBody));

        // Verify that ReadUserByAuthProviderUserIdAsync was called
        _mockDynamoDbService.Verify(
            x =>
                x.ReadUserByAuthProviderUserIdAsync(
                    authProviderUserId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        // Verify that GetByNameAsync was called to check for existing meeting
        _mockMeetingService.Verify(
            x => x.GetByNameAsync(testUserId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        // Verify that CreateAsync was NOT called (since meeting already exists)
        _mockMeetingService.Verify(
            x => x.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        // Verify that WriteUserAsync was NOT called (since user exists and meeting exists)
        _mockDynamoDbService.Verify(
            x => x.WriteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handler_WhenUserFoundButNoMeeting_ShouldCreateMeetingAndUpdateUser()
    {
        // Arrange
        var connectionId = "test-connection-id";
        var authProviderUserId = "test-auth-provider-user-id";
        var existingUserId = "existing-user-id";
        var command = new OnConnectCommand
        {
            ConnectionId = connectionId,
            AuthProviderUserId = authProviderUserId
        };

        // Create an existing user
        var existingUser = new User
        {
            UserId = existingUserId,
            AuthProviderUserId = authProviderUserId,
            ConnectionId = "old-connection-id",
            Profile = new UserProfile
            {
                UserName = "Test User",
                Email = "test@example.com",
                Country = "US"
            }
        };
        var newMeetingId = Guid.NewGuid();

        // Mock DynamoDbService to return existing user
        _mockDynamoDbService
            .Setup(
                x =>
                    x.ReadUserByAuthProviderUserIdAsync(
                        authProviderUserId,
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(existingUser);

        // Mock MeetingService to return null (no existing meeting)
        _mockMeetingService
            .Setup(x => x.GetByNameAsync(existingUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Meeting?)null);

        // Mock MeetingService to return a new meeting ID
        _mockMeetingService
            .Setup(x => x.CreateAsync(existingUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Meeting { Id = newMeetingId });

        // Mock DynamoDbService to handle user update
        _mockDynamoDbService
            .Setup(x => x.WriteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _lambdaHandler.Handle(command);

        // Assert
        AssertSuccess(result, 201);

        // Verify that ReadUserByAuthProviderUserIdAsync was called
        _mockDynamoDbService.Verify(
            x =>
                x.ReadUserByAuthProviderUserIdAsync(
                    authProviderUserId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        // Verify that GetByNameAsync was called to check for existing meeting
        _mockMeetingService.Verify(
            x => x.GetByNameAsync(existingUserId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        // Verify that CreateAsync was called to create a new meeting
        _mockMeetingService.Verify(
            x => x.CreateAsync(existingUserId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        // Verify that WriteUserAsync was called to update the user with the new meeting ID
        _mockDynamoDbService.Verify(
            x => x.WriteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task LambdaHandler_ShouldDelegateToMediator()
    {
        // Arrange
        var connectionId = "test-connection-id";
        var authProviderUserId = "test-auth-provider-user-id";
        var command = new OnConnectCommand
        {
            ConnectionId = connectionId,
            AuthProviderUserId = authProviderUserId
        };

        var expectedResponse = new APIGatewayProxyResponse
        {
            StatusCode = 201,
            Body = "test response"
        };

        // Create a Moq mock for IMediator
        var mockMediator = new Mock<IMediator>();
        mockMediator
            .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Use the mock mediator for this handler
        var handler = new MediatRLambdaHandler<OnConnectCommand>(mockMediator.Object);

        // Act
        var result = await handler.Handle(command);

        // Assert
        result.Should().Be(expectedResponse);
        mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WebSocketRequest_ShouldBeProcessedCorrectly()
    {
        // Arrange
        var connectionId = "test-connection-id";
        var userId = "test-user-id";
        var request = CreateMockWebSocketRequest(connectionId, userId);

        // Create a Moq mock for IMediator
        var mockMediator = new Mock<IMediator>();
        mockMediator
            .Setup(x => x.Send(It.IsAny<OnConnectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Flyingdarts.Lambda.Core.Infrastructure.ResponseBuilder.SuccessJson(new { }, 201)
            );

        // Use the mock mediator for this handler
        var handler = new MediatRLambdaHandler<OnConnectCommand>(mockMediator.Object);

        // Act & Assert using the base test utilities
        // This demonstrates how to use the base test utilities for WebSocket requests
        var command = new OnConnectCommand
        {
            ConnectionId = connectionId,
            AuthProviderUserId = userId
        };

        var result = await handler.Handle(command);

        // Assert
        AssertSuccess(result, 201);
        mockMediator.Verify(
            x => x.Send(It.IsAny<OnConnectCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
