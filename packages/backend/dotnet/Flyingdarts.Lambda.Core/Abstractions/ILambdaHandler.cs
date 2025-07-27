namespace Flyingdarts.Lambda.Core.Abstractions;

/// <summary>
/// Interface for Lambda handlers that process requests and return responses
/// </summary>
/// <typeparam name="TRequest">The type of request to handle</typeparam>
/// <typeparam name="TResponse">The type of response to return</typeparam>
public interface ILambdaHandler<in TRequest, TResponse>
{
    /// <summary>
    /// Handles the request and returns a response
    /// </summary>
    /// <param name="request">The request to handle</param>
    /// <returns>The response</returns>
    Task<TResponse> Handle(TRequest request);
}

/// <summary>
/// Interface for Lambda handlers that process requests and return APIGatewayProxyResponse
/// </summary>
/// <typeparam name="TRequest">The type of request to handle</typeparam>
public interface ILambdaHandler<in TRequest> : ILambdaHandler<TRequest, APIGatewayProxyResponse> { }
