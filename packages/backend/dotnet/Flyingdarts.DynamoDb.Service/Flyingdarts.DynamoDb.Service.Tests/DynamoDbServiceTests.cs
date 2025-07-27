using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Flyingdarts.Persistence;
using Microsoft.Extensions.Options;
using Moq;

namespace Flyingdarts.DynamoDb.Service.Tests;


public class DynamoDbServiceTests
{
    [Fact]
    public async Task ReadUserByAuthProviderUserIdAsync_WhenQueryResultIsNull_ThrowsUserNotFoundException()
    {
        // Arrange
        var mockDbContext = new Mock<IDynamoDBContext>();
        var mockApplicationOptions = new Mock<IOptions<ApplicationOptions>>();
        var mockApplicationOptionsValue = new ApplicationOptions();

        mockApplicationOptions.Setup(x => x.Value).Returns(mockApplicationOptionsValue);

        // Mock the query to return an empty list (simulating null/empty result)
        var mockAsyncSearch = new Mock<IAsyncSearch<User>>();
        mockAsyncSearch
            .Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        mockDbContext
            .Setup(
                x =>
                    x.FromQueryAsync<User>(
                        It.IsAny<QueryOperationConfig>(),
                        It.IsAny<DynamoDBOperationConfig>()
                    )
            )
            .Returns(mockAsyncSearch.Object);

        var dynamoDbService = new DynamoDbService(
            mockDbContext.Object,
            mockApplicationOptions.Object
        );
        var authProviderUserId = "test-auth-provider-user-id";
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DynamoDbService.UserNotFoundException>(
            () =>
                dynamoDbService.ReadUserByAuthProviderUserIdAsync(
                    authProviderUserId,
                    cancellationToken
                )
        );

        // Verify the exception details
        Assert.Equal("authProviderUserId", exception.SearchParam);
        Assert.Equal(authProviderUserId, exception.UserId);
        Assert.Contains(
            $"User not found by authProviderUserId: {authProviderUserId}",
            exception.Message
        );
    }

    [Fact]
    public async Task ReadUserByAuthProviderUserIdAsync_WhenQueryResultIsEmpty_ThrowsUserNotFoundException()
    {
        // Arrange
        var mockDbContext = new Mock<IDynamoDBContext>();
        var mockApplicationOptions = new Mock<IOptions<ApplicationOptions>>();
        var mockApplicationOptionsValue = new ApplicationOptions();

        mockApplicationOptions.Setup(x => x.Value).Returns(mockApplicationOptionsValue);

        // Mock the query to return an empty list
        var mockAsyncSearch = new Mock<IAsyncSearch<User>>();
        mockAsyncSearch
            .Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        mockDbContext
            .Setup(
                x =>
                    x.FromQueryAsync<User>(
                        It.IsAny<QueryOperationConfig>(),
                        It.IsAny<DynamoDBOperationConfig>()
                    )
            )
            .Returns(mockAsyncSearch.Object);

        var dynamoDbService = new DynamoDbService(
            mockDbContext.Object,
            mockApplicationOptions.Object
        );
        var authProviderUserId = "non-existent-user-id";
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DynamoDbService.UserNotFoundException>(
            () =>
                dynamoDbService.ReadUserByAuthProviderUserIdAsync(
                    authProviderUserId,
                    cancellationToken
                )
        );

        // Verify the exception details
        Assert.Equal("authProviderUserId", exception.SearchParam);
        Assert.Equal(authProviderUserId, exception.UserId);
        Assert.Contains(
            $"User not found by authProviderUserId: {authProviderUserId}",
            exception.Message
        );
    }

    [Fact]
    public async Task ReadUserByAuthProviderUserIdAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var mockDbContext = new Mock<IDynamoDBContext>();
        var mockApplicationOptions = new Mock<IOptions<ApplicationOptions>>();
        var mockApplicationOptionsValue = new ApplicationOptions();

        mockApplicationOptions.Setup(x => x.Value).Returns(mockApplicationOptionsValue);

        // Mock the query to return a user
        var existingUser = new User
        {
            UserId = "test-user-id",
            AuthProviderUserId = "test-auth-provider-user-id",
            Profile = new UserProfile
            {
                UserName = "Test User",
                Email = "test@example.com",
                Country = "US"
            }
        };
        var queryResult = new List<User> { existingUser };

        var mockAsyncSearch = new Mock<IAsyncSearch<User>>();
        mockAsyncSearch
            .Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        mockDbContext
            .Setup(
                x =>
                    x.FromQueryAsync<User>(
                        It.IsAny<QueryOperationConfig>(),
                        It.IsAny<DynamoDBOperationConfig>()
                    )
            )
            .Returns(mockAsyncSearch.Object);

        var dynamoDbService = new DynamoDbService(
            mockDbContext.Object,
            mockApplicationOptions.Object
        );
        var authProviderUserId = "test-auth-provider-user-id";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await dynamoDbService.ReadUserByAuthProviderUserIdAsync(
            authProviderUserId,
            cancellationToken
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingUser.UserId, result.UserId);
        Assert.Equal(existingUser.AuthProviderUserId, result.AuthProviderUserId);
        Assert.Equal(existingUser.Profile.UserName, result.Profile.UserName);
    }
}
