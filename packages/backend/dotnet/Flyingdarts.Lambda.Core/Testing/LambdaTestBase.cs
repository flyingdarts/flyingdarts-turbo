namespace Flyingdarts.Lambda.Core.Testing;

/// <summary>
/// Base class for Lambda function tests that provides common testing utilities
/// </summary>
/// <typeparam name="TRequest">The type of request the Lambda handles</typeparam>
/// <typeparam name="TResponse">The type of response the Lambda returns</typeparam>
public abstract class LambdaTestBase<TRequest, TResponse>
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Sets up the test environment with a mock service provider
    /// </summary>
    /// <param name="configureServices">Action to configure mock services</param>
    protected virtual void Setup(Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();

        // Configure common services with mocks
        ConfigureMockServices(services);

        // Allow derived classes to configure additional services
        configureServices?.Invoke(services);

        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Creates a mock API Gateway request
    /// </summary>
    /// <param name="httpMethod">The HTTP method</param>
    /// <param name="path">The request path</param>
    /// <param name="body">The request body</param>
    /// <param name="headers">The request headers</param>
    /// <param name="userId">The user ID from the authorizer</param>
    /// <returns>The mock API Gateway request</returns>
    protected static APIGatewayProxyRequest CreateMockRequest(
        string httpMethod = "GET",
        string path = "/test",
        string? body = null,
        Dictionary<string, string>? headers = null,
        string? userId = null
    )
    {
        var request = new APIGatewayProxyRequest
        {
            HttpMethod = httpMethod,
            Path = path,
            Body = body,
            Headers = headers ?? new Dictionary<string, string>(),
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = Guid.NewGuid().ToString(),
                Authorizer =
                    userId != null
                        ? new APIGatewayCustomAuthorizerContext { { "UserId", userId } }
                        : null
            }
        };

        return request;
    }

    /// <summary>
    /// Creates a mock WebSocket request
    /// </summary>
    /// <param name="connectionId">The connection ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>The mock API Gateway request</returns>
    protected static APIGatewayProxyRequest CreateMockWebSocketRequest(
        string? connectionId = null,
        string? userId = null
    )
    {
        return new APIGatewayProxyRequest
        {
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                ConnectionId = connectionId ?? Guid.NewGuid().ToString(),
                Authorizer =
                    userId != null
                        ? new APIGatewayCustomAuthorizerContext { { "UserId", userId } }
                        : null
            }
        };
    }

    /// <summary>
    /// Asserts that the response is successful
    /// </summary>
    /// <param name="response">The response to check</param>
    /// <param name="expectedStatusCode">The expected status code (default: 200)</param>
    protected static void AssertSuccess(
        APIGatewayProxyResponse response,
        int expectedStatusCode = 200
    )
    {
        if (response == null)
            throw new ArgumentNullException(nameof(response), "Response cannot be null");

        if (response.StatusCode != expectedStatusCode)
            throw new InvalidOperationException(
                $"Expected status code {expectedStatusCode}, but got {response.StatusCode}"
            );

        if (
            response.Headers == null
            || !response.Headers.ContainsKey("Access-Control-Allow-Origin")
        )
            throw new InvalidOperationException("Response must include CORS headers");
    }

    /// <summary>
    /// Asserts that the response is an error
    /// </summary>
    /// <param name="response">The response to check</param>
    /// <param name="expectedStatusCode">The expected status code</param>
    protected static void AssertError(APIGatewayProxyResponse response, int expectedStatusCode)
    {
        if (response == null)
            throw new ArgumentNullException(nameof(response), "Response cannot be null");

        if (response.StatusCode != expectedStatusCode)
            throw new InvalidOperationException(
                $"Expected status code {expectedStatusCode}, but got {response.StatusCode}"
            );

        if (string.IsNullOrEmpty(response.Body) || !response.Body.Contains("error"))
            throw new InvalidOperationException(
                "Error response must include error message in body"
            );
    }

    /// <summary>
    /// Configures mock services for testing
    /// </summary>
    /// <param name="services">The service collection</param>
    protected virtual void ConfigureMockServices(IServiceCollection services)
    {
        // Configure real MediatR for testing
        services.AddMediatR(
            cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(LambdaTestBase<TRequest, TResponse>).Assembly
                )
        );

        // Configure real FluentValidation for testing
        services.AddValidatorsFromAssemblyContaining(typeof(LambdaTestBase<TRequest, TResponse>));
    }
}
