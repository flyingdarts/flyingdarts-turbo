using System.Text;
using System.Text.Json;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Flyingdarts.Core.Models;
using Flyingdarts.NotifyRooms.Service.Interfaces;

namespace Flyingdarts.NotifyRooms.Service.Services;

public class NotifyRoomsService : INotifyRoomsService
{
    private readonly IAmazonApiGatewayManagementApi _apiGatewayClient;

    public NotifyRoomsService(IAmazonApiGatewayManagementApi apiGatewayClient)
    {
        _apiGatewayClient = apiGatewayClient;
    }

    public async Task NotifyRoomAsync<TNotification>(
        SocketMessage<TNotification> request,
        string[] connectionIds,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Console.WriteLine($"[NotifyRoomsService] Notifying connections: [{string.Join(", ", connectionIds)}]");

            // Create a timeout cancellation token to prevent hanging
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(20)); // 20 second timeout
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            // Serialize the message once
            var messageJson = JsonSerializer.Serialize(request);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            // Process connections sequentially to avoid .NET 8 concurrency issues
            var successfulCount = 0;
            var failedCount = 0;

            foreach (var connectionId in connectionIds)
            {
                if (string.IsNullOrEmpty(connectionId) || connectionId == request.ConnectionId)
                    continue;

                try
                {
                    // Create a NEW stream for each connection to avoid position issues in .NET 8
                    var messageStream = new MemoryStream(messageBytes);

                    var postConnectionRequest = new PostToConnectionRequest { ConnectionId = connectionId, Data = messageStream };

                    await _apiGatewayClient.PostToConnectionAsync(postConnectionRequest, combinedCts.Token);

                    successfulCount++;
                    Console.WriteLine($"[NotifyRoomsService] Successfully sent message to connection: {connectionId}");
                }
                catch (GoneException)
                {
                    // Connection is no longer available, log and continue
                    Console.WriteLine($"[NotifyRoomsService] Connection {connectionId} is no longer available");
                    failedCount++;
                }
                catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
                {
                    Console.WriteLine($"[NotifyRoomsService] Timeout sending to connection {connectionId}");
                    failedCount++;
                    break; // Exit the loop on timeout
                }
                catch (Exception ex)
                {
                    // Log other errors but don't fail the entire operation
                    Console.WriteLine($"[NotifyRoomsService] Error sending to connection {connectionId}: {ex.Message}");
                    failedCount++;
                }
            }

            Console.WriteLine($"[NotifyRoomsService] Completed: {successfulCount} successful, {failedCount} failed");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("[NotifyRoomsService] Operation was cancelled by caller");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[NotifyRoomsService] Error processing request: {e.Message}");
            throw; // Re-throw the exception to ensure proper error handling
        }
        finally
        {
            Console.WriteLine($"[NotifyRoomsService] Done processing request: {JsonSerializer.Serialize(request)}");
        }
    }
}
