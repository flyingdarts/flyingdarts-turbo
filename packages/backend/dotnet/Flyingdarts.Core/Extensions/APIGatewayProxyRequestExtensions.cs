using System.Text;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Flyingdarts.Core.Models;

namespace Flyingdarts.Core.Extensions;

/// <summary>
/// Extension methods for APIGatewayProxyRequest to convert to SocketMessage.
/// </summary>
public static class APIGatewayProxyRequestExtensions
{
    /// <summary>
    /// Converts an APIGatewayProxyRequest to a SocketMessage of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the message payload.</typeparam>
    /// <param name="request">The API Gateway proxy request to convert.</param>
    /// <param name="serializer">The Lambda serializer to use for deserialization.</param>
    /// <returns>A SocketMessage instance if the request body is valid; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request or serializer is null.</exception>
    public static SocketMessage<T>? To<T>(
        this APIGatewayProxyRequest request,
        ILambdaSerializer serializer
    ) where T : class
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(serializer);

        // Early return for empty or whitespace body
        if (string.IsNullOrWhiteSpace(request.Body))
            return null;

        try
        {
            // Use ReadOnlySpan<byte> for better performance when possible
            var bodyBytes = Encoding.UTF8.GetBytes(request.Body);
            using var memoryStream = new MemoryStream(bodyBytes);
            var socketMessage = serializer.Deserialize<SocketMessage<T>>(memoryStream);

            // Set the connection ID from the request context
            if (socketMessage != null)
            {
                socketMessage.ConnectionId = request.RequestContext?.ConnectionId;
            }

            return socketMessage;
        }
        catch (Exception ex) when (ex is JsonException || ex is InvalidOperationException)
        {
            // Log the exception details for debugging (in a real implementation, you might want to use ILogger)
            // For now, we'll return null to maintain the existing behavior
            return null;
        }
    }
}