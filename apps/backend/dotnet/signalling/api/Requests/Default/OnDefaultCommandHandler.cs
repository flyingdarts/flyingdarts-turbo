using System.Text;
using System.Text.Json;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Core.Models;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Lambda.Core.Infrastructure;
using Flyingdarts.Persistence;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api.Requests.Default;

public class OnDefaultCommandHandler : IRequestHandler<OnDefaultCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDbService _dynamoDbService;

    public OnDefaultCommandHandler(IDynamoDbService dynamoDbService)
    {
        _dynamoDbService = dynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(
        OnDefaultCommand request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[OnDefault] Starting default message broadcast");

        try
        {
            // Get all users with connection IDs
            var allUsers = await GetAllUsersWithConnections(cancellationToken);
            var connectedUsers = allUsers
                .Where(u => !string.IsNullOrEmpty(u.ConnectionId))
                .ToList();

            Console.WriteLine(
                $"[OnDefault] Found {connectedUsers.Count} connected users to broadcast to"
            );

            // Construct the socket message
            var socketMessage = new SocketMessage<OnDefaultCommand>
            {
                Message = request,
                Action = "default$"
            };

            var messageJson = JsonSerializer.Serialize(socketMessage);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            // Loop through all connected users and broadcast the message
            var successCount = 0;
            foreach (var user in connectedUsers)
            {
                try
                {
                    // Create a new stream for each connection to avoid position issues
                    var stream = new MemoryStream(messageBytes);
                    var postConnectionRequest = new PostToConnectionRequest
                    {
                        ConnectionId = user.ConnectionId,
                        Data = stream
                    };

                    Console.WriteLine(
                        $"[OnDefault] Broadcasting to connection {successCount}: {user.ConnectionId}"
                    );
                    await request
                        .ApiGatewayManagementApiClient
                        .PostToConnectionAsync(postConnectionRequest, cancellationToken);
                    successCount++;
                }
                catch (GoneException)
                {
                    // Connection is no longer available, remove the connection ID
                    Console.WriteLine(
                        $"[OnDefault] Connection {user.ConnectionId} is no longer available, removing connection ID"
                    );
                    user.ConnectionId = null;
                    await _dynamoDbService.WriteUserAsync(user, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log other errors but don't fail the entire operation
                    Console.WriteLine(
                        $"[OnDefault] Error sending to connection {user.ConnectionId}: {ex.Message}"
                    );
                }
            }

            Console.WriteLine(
                $"[OnDefault] Successfully broadcasted to {successCount} connections"
            );

            return ResponseBuilder.SuccessJson(socketMessage, 200);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OnDefault] Error during default message broadcast: {ex.Message}");
            Console.WriteLine($"[OnDefault] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task<List<User>> GetAllUsersWithConnections(CancellationToken cancellationToken)
    {
        // Get all users and filter by those with connection IDs
        var allUsers = await _dynamoDbService.ReadAllUsersAsync(cancellationToken);
        var connectedUsers = allUsers.Where(u => !string.IsNullOrEmpty(u.ConnectionId)).ToList();

        Console.WriteLine(
            $"[OnDefault] Found {allUsers.Count} total users, {connectedUsers.Count} with active connections"
        );

        return connectedUsers;
    }
}
