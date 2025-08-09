// using System.Text;
// using System.Text.Json;
// using Amazon.ApiGatewayManagementApi;
// using Amazon.ApiGatewayManagementApi.Model;
// using Flyingdarts.Core.Models;
// using Flyingdarts.NotifyRooms.Service.Services;
// using Moq;

// namespace Flyingdarts.NotifyRooms.Service.Tests;

// public class NotifyRoomsServiceTests
// {
//     class Player
//     {
//         public string Id { get; set; }
//     }

//     class NotifyRoomsTestRequest
//     {
//         public string ConnectionId { get; set; }
//     }

//     [Fact]
//     public async Task NotifyRoomAsync_ShouldCallPostToConnectionAsync_WithCorrectParameters()
//     {
//         // Arrange
//         var mockApiGatewayClient = new Mock<IAmazonApiGatewayManagementApi>();
//         var service = new NotifyRoomsService(mockApiGatewayClient.Object);

//         var testNotification = new NotifyRoomsTestRequest { ConnectionId = "Hablabla" };
//         var socketMessage = new SocketMessage<NotifyRoomsTestRequest>
//         {
//             Action = "test-action",
//             Message = testNotification
//         };

//         var connectionIds = new[] { "test-connection-id-1", "test-connection-id-2" };
//         var cancellationToken = CancellationToken.None;

//         mockApiGatewayClient
//             .Setup(
//                 x =>
//                     x.PostToConnectionAsync(
//                         It.IsAny<PostToConnectionRequest>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ReturnsAsync(new PostToConnectionResponse());

//         // Act
//         await service.NotifyRoomAsync(socketMessage, connectionIds, cancellationToken);

//         // Assert
//         mockApiGatewayClient.Verify(
//             x =>
//                 x.PostToConnectionAsync(
//                     It.Is<PostToConnectionRequest>(
//                         req =>
//                             req.ConnectionId == connectionIds[0]
//                             && // Currently only sends to first connection
//                             req.Data != null
//                     ),
//                     cancellationToken
//                 ),
//             Times.Once,
//             "PostToConnectionAsync should be called exactly once with the correct parameters"
//         );
//     }

//     [Fact]
//     public async Task NotifyRoomAsync_ShouldSerializeMessageCorrectly()
//     {
//         // Arrange
//         var mockApiGatewayClient = new Mock<IAmazonApiGatewayManagementApi>();
//         var service = new NotifyRoomsService(mockApiGatewayClient.Object);

//         var testNotification = new NotifyRoomsTestRequest { ConnectionId = "Test notification" };
//         var socketMessage = new SocketMessage<NotifyRoomsTestRequest>
//         {
//             Action = "test-serialization",
//             Message = testNotification,
//             Metadata = new Dictionary<string, object>
//             {
//                 { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
//             }
//         };

//         var connectionIds = new[] { "test-connection-id" };
//         var cancellationToken = CancellationToken.None;

//         PostToConnectionRequest capturedRequest = null;
//         mockApiGatewayClient
//             .Setup(
//                 x =>
//                     x.PostToConnectionAsync(
//                         It.IsAny<PostToConnectionRequest>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .Callback<PostToConnectionRequest, CancellationToken>(
//                 (req, ct) => capturedRequest = req
//             )
//             .ReturnsAsync(new PostToConnectionResponse());

//         // Act
//         await service.NotifyRoomAsync(socketMessage, connectionIds, cancellationToken);

//         // Assert
//         Assert.NotNull(capturedRequest);
//         Assert.NotNull(capturedRequest.Data);

//         // Verify the serialized data matches our expected JSON
//         var expectedJson = JsonSerializer.Serialize(socketMessage);
//         var actualBytes = new byte[capturedRequest.Data.Length];
//         capturedRequest.Data.Read(actualBytes, 0, actualBytes.Length);
//         var actualJson = Encoding.UTF8.GetString(actualBytes);

//         Assert.Equal(expectedJson, actualJson);
//     }

//     [Fact]
//     public async Task NotifyRoomAsync_ShouldHandleExceptions_WithoutThrowing()
//     {
//         // Arrange
//         var mockApiGatewayClient = new Mock<IAmazonApiGatewayManagementApi>();
//         var service = new NotifyRoomsService(mockApiGatewayClient.Object);

//         var socketMessage = new SocketMessage<NotifyRoomsTestRequest> { };

//         var connectionIds = new[] { "test-connection-id" };
//         var cancellationToken = CancellationToken.None;

//         mockApiGatewayClient
//             .Setup(
//                 x =>
//                     x.PostToConnectionAsync(
//                         It.IsAny<PostToConnectionRequest>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .ThrowsAsync(new Exception("API Gateway error"));

//         // Act & Assert
//         var exception = await Record.ExceptionAsync(
//             () => service.NotifyRoomAsync(socketMessage, connectionIds, cancellationToken)
//         );

//         Assert.Null(exception); // Should not throw, exceptions are caught and logged
//         mockApiGatewayClient.Verify(
//             x =>
//                 x.PostToConnectionAsync(
//                     It.IsAny<PostToConnectionRequest>(),
//                     It.IsAny<CancellationToken>()
//                 ),
//             Times.Once
//         );
//     }

//     [Fact]
//     public async Task NotifyRoomAsync_WithMultipleConnectionIds_ShouldCallPostToConnectionForEachConnection()
//     {
//         // Arrange
//         var mockApiGatewayClient = new Mock<IAmazonApiGatewayManagementApi>();
//         var service = new NotifyRoomsService(mockApiGatewayClient.Object);

//         var socketMessage = new SocketMessage<object>
//         {
//             Action = "test-multiple-connections",
//             Message = new
//             {
//                 Content = "Broadcasting to multiple connections",
//                 Timestamp = DateTime.UtcNow
//             }
//         };

//         var connectionIds = new[] { "conn-1", "conn-2", "conn-3", "conn-4" };
//         var cancellationToken = CancellationToken.None;

//         // Track all calls to verify each connection ID
//         var capturedRequests = new List<PostToConnectionRequest>();
//         mockApiGatewayClient
//             .Setup(
//                 x =>
//                     x.PostToConnectionAsync(
//                         It.IsAny<PostToConnectionRequest>(),
//                         It.IsAny<CancellationToken>()
//                     )
//             )
//             .Callback<PostToConnectionRequest, CancellationToken>(
//                 (req, ct) => capturedRequests.Add(req)
//             )
//             .ReturnsAsync(new PostToConnectionResponse());

//         // Act
//         await service.NotifyRoomAsync(socketMessage, connectionIds, cancellationToken);

//         // Assert
//         // Verify PostToConnectionAsync was called exactly once for each connection
//         mockApiGatewayClient.Verify(
//             x =>
//                 x.PostToConnectionAsync(
//                     It.IsAny<PostToConnectionRequest>(),
//                     It.IsAny<CancellationToken>()
//                 ),
//             Times.Exactly(connectionIds.Length),
//             $"PostToConnectionAsync should be called exactly {connectionIds.Length} times for {connectionIds.Length} connections"
//         );

//         // Verify each connection ID was used exactly once
//         Assert.Equal(connectionIds.Length, capturedRequests.Count);
//         var capturedConnectionIds = capturedRequests.Select(req => req.ConnectionId).ToArray();

//         foreach (var expectedConnectionId in connectionIds)
//         {
//             Assert.Contains(expectedConnectionId, capturedConnectionIds);
//         }

//         // Verify no duplicate connection IDs were sent
//         Assert.Equal(connectionIds.Length, capturedConnectionIds.Distinct().Count());
//     }

//     [Fact]
//     public async Task NotifyRoomAsync_WithEmptyConnectionIds_ShouldNotCallPostToConnection()
//     {
//         // Arrange
//         var mockApiGatewayClient = new Mock<IAmazonApiGatewayManagementApi>();
//         var service = new NotifyRoomsService(mockApiGatewayClient.Object);

//         var socketMessage = new SocketMessage<object>
//         {
//             Action = "test-empty-connections",
//             Message = new { Content = "Test" }
//         };

//         var connectionIds = new string[] { }; // Empty array
//         var cancellationToken = CancellationToken.None;

//         // Act
//         await service.NotifyRoomAsync(socketMessage, connectionIds, cancellationToken);

//         // Assert
//         mockApiGatewayClient.Verify(
//             x =>
//                 x.PostToConnectionAsync(
//                     It.IsAny<PostToConnectionRequest>(),
//                     It.IsAny<CancellationToken>()
//                 ),
//             Times.Never,
//             "PostToConnectionAsync should not be called when no connection IDs are provided"
//         );
//     }
// }
