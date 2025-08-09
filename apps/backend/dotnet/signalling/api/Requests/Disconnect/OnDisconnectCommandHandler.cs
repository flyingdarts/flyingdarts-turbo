using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.DynamoDb.Service;
using Flyingdarts.Lambda.Core.Infrastructure;
using MediatR;

namespace Flyingdarts.Backend.Signalling.Api.Requests.Disconnect;

public class OnDisconnectCommandHandler : IRequestHandler<OnDisconnectCommand, APIGatewayProxyResponse>
{
    private readonly IDynamoDbService _dynamoDbService;

    public OnDisconnectCommandHandler(IDynamoDbService dynamoDbService)
    {
        _dynamoDbService = dynamoDbService;
    }

    public async Task<APIGatewayProxyResponse> Handle(OnDisconnectCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[OnDisconnect] Starting disconnect process for ConnectionId: {request.ConnectionId}");

        try
        {
            // Find user by connection ID
            var user = await _dynamoDbService.ReadUserByConnectionIdAsync(request.ConnectionId, cancellationToken);

            if (user != null)
            {
                Console.WriteLine($"[OnDisconnect] Found user for ConnectionId: {request.ConnectionId}, UserId: {user.UserId}");

                // Remove the connection ID
                user.ConnectionId = null;
                await _dynamoDbService.WriteUserAsync(user, cancellationToken);

                Console.WriteLine($"[OnDisconnect] Successfully removed connection ID for UserId: {user.UserId}");
            }
            else
            {
                Console.WriteLine($"[OnDisconnect] No user found for ConnectionId: {request.ConnectionId}");
            }

            return ResponseBuilder.SuccessJson(new { message = "Disconnected." }, 200);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[OnDisconnect] Error during disconnect process for ConnectionId: {request.ConnectionId}. Error: {ex.Message}"
            );
            Console.WriteLine($"[OnDisconnect] Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
