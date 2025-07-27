namespace Flyingdarts.Lambda.Core.Infrastructure;

/// <summary>
/// Utility class for building standardized API Gateway responses
/// </summary>
public static class ResponseBuilder
{
    /// <summary>
    /// Creates a successful response with the specified body
    /// </summary>
    /// <param name="body">The response body</param>
    /// <param name="statusCode">The HTTP status code (default: 200)</param>
    /// <returns>The API Gateway response</returns>
    public static APIGatewayProxyResponse Success(string body, int statusCode = 200)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = body,
            Headers = GetDefaultHeaders()
        };
    }

    /// <summary>
    /// Creates a successful JSON response with the specified object
    /// </summary>
    /// <param name="data">The data to serialize</param>
    /// <param name="statusCode">The HTTP status code (default: 200)</param>
    /// <returns>The API Gateway response</returns>
    public static APIGatewayProxyResponse SuccessJson(object data, int statusCode = 200)
    {
        return Success(JsonSerializer.Serialize(data), statusCode);
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="statusCode">The HTTP status code (default: 400)</param>
    /// <returns>The API Gateway response</returns>
    public static APIGatewayProxyResponse Error(string message, int statusCode = 400)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(new { error = message }),
            Headers = GetDefaultHeaders()
        };
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    /// <param name="message">The not found message (default: "Resource not found")</param>
    /// <returns>The API Gateway response</returns>
    public static APIGatewayProxyResponse NotFound(string message = "Resource not found")
    {
        return Error(message, 404);
    }

    /// <summary>
    /// Creates an internal server error response
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>The API Gateway response</returns>
    public static APIGatewayProxyResponse InternalServerError(Exception exception)
    {
        var errorResponse = new
        {
            error = "Internal server error",
            message = exception.Message,
            stackTrace = exception.StackTrace
        };

        return new APIGatewayProxyResponse
        {
            StatusCode = 500,
            Body = JsonSerializer.Serialize(errorResponse),
            Headers = GetDefaultHeaders()
        };
    }

    /// <summary>
    /// Gets the default CORS headers
    /// </summary>
    /// <returns>The default headers dictionary</returns>
    private static Dictionary<string, string> GetDefaultHeaders()
    {
        return new Dictionary<string, string>
        {
            { "Access-Control-Allow-Origin", "*" },
            { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
            { "Access-Control-Allow-Headers", "Content-Type,Authorization" },
            { "Content-Type", "application/json" }
        };
    }
}
