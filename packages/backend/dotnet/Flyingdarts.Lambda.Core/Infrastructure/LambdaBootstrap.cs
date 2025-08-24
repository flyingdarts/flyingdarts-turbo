using Flyingdarts.Lambda.Core.Abstractions;

namespace Flyingdarts.Lambda.Core.Infrastructure;

/// <summary>
/// Base class for Lambda function bootstrap that provides common setup and execution logic
/// </summary>
/// <typeparam name="TRequest">The type of request the Lambda handles</typeparam>
/// <typeparam name="TResponse">The type of response the Lambda returns</typeparam>
public abstract class LambdaBootstrap<TRequest, TResponse>
{
    private readonly ILambdaHandler<TRequest, TResponse> _handler;
    private readonly DefaultLambdaJsonSerializer _serializer;

    protected LambdaBootstrap(ILambdaHandler<TRequest, TResponse> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);
    }

    /// <summary>
    /// Runs the Lambda function
    /// </summary>
    public async Task RunAsync()
    {
        var handler = CreateHandler();
        await LambdaBootstrapBuilder.Create(handler, _serializer).Build().RunAsync();
    }

    /// <summary>
    /// Creates the Lambda handler function
    /// </summary>
    /// <returns>The handler function</returns>
    protected abstract Func<TRequest, Task<TResponse>> CreateHandler();
}

/// <summary>
/// Base class for Lambda function bootstrap that handles APIGatewayProxyRequest and returns APIGatewayProxyResponse
/// </summary>
/// <typeparam name="TRequest">The type of request the Lambda handles</typeparam>
public abstract class ApiGatewayLambdaBootstrap<TRequest>
    : LambdaBootstrap<APIGatewayProxyRequest, APIGatewayProxyResponse>
{
    private readonly ILambdaHandler<TRequest> _innerHandler;

    protected ApiGatewayLambdaBootstrap(ILambdaHandler<TRequest> innerHandler)
        : base(new ApiGatewayHandler<TRequest>(innerHandler))
    {
        _innerHandler = innerHandler;
    }

    /// <summary>
    /// Creates the Lambda handler function that converts APIGatewayProxyRequest to the specific request type
    /// </summary>
    /// <returns>The handler function</returns>
    protected override Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>> CreateHandler()
    {
        return async (request) =>
        {
            var convertedRequest = ConvertRequest(request);
            return await _innerHandler.Handle(convertedRequest);
        };
    }

    /// <summary>
    /// Converts the APIGatewayProxyRequest to the specific request type
    /// </summary>
    /// <param name="request">The API Gateway request</param>
    /// <returns>The converted request</returns>
    protected abstract TRequest ConvertRequest(APIGatewayProxyRequest request);

    private class ApiGatewayHandler<T>
        : ILambdaHandler<APIGatewayProxyRequest, APIGatewayProxyResponse>
    {
        private readonly ILambdaHandler<T> _innerHandler;

        public ApiGatewayHandler(ILambdaHandler<T> innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
        {
            // This is a placeholder - the actual conversion happens in the derived class
            return Task.FromException<APIGatewayProxyResponse>(
                new NotImplementedException("This should not be called directly")
            );
        }
    }
}
