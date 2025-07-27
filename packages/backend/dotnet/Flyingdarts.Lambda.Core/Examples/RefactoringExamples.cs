namespace Flyingdarts.Lambda.Core.Examples;

/// <summary>
/// Examples showing how to refactor existing Lambda functions to use the core package
/// </summary>
public static class RefactoringExamples
{
    /// <summary>
    /// Example 1: Refactoring OnConnect Lambda Function.cs
    /// </summary>
    public static class OnConnectRefactoring
    {
        // BEFORE: Original Function.cs
        public static class Before
        {
            /*
            var services = ServiceFactory.GetServiceProvider();
            var innerHandler = new InnerHandler(services.GetRequiredService<IMediator>());
            var serializer = new DefaultLambdaJsonSerializer(x => x.PropertyNameCaseInsensitive = true);

            var handler = async (APIGatewayProxyRequest request) =>
            {
                var socketRequest = new SocketMessage<OnConnectCommand>
                {
                    Message = new OnConnectCommand
                    {
                        ConnectionId = request.RequestContext.ConnectionId,
                        AuthProviderUserId = request.RequestContext.Authorizer?.GetValueOrDefault("UserId").ToString()
                    }
                };
                return await innerHandler.Handle(socketRequest);
            };

            await LambdaBootstrapBuilder.Create(handler, serializer).Build().RunAsync();
            */
        }

        // AFTER: Refactored Function.cs using core package
        public static class After
        {
            /*
            using Flyingdarts.Lambda.Core.Infrastructure;
            using Flyingdarts.Lambda.Core.Handlers;
            using Flyingdarts.Backend.Signalling.OnConnect;
            using Flyingdarts.Backend.Signalling.OnConnect.CQRS;

            var services = ServiceFactory.GetServiceProvider();
            var handler = new MediatRLambdaHandler<OnConnectCommand>(services.GetRequiredService<IMediator>());

            var bootstrap = new OnConnectBootstrap(handler);
            await bootstrap.RunAsync();

            // Define the bootstrap class
            public class OnConnectBootstrap : ApiGatewayLambdaBootstrap<OnConnectCommand>
            {
                public OnConnectBootstrap(ILambdaHandler<OnConnectCommand> handler) : base(handler) { }

                protected override OnConnectCommand ConvertRequest(APIGatewayProxyRequest request)
                {
                    return new OnConnectCommand
                    {
                        ConnectionId = request.RequestContext.ConnectionId,
                        AuthProviderUserId = request.RequestContext.Authorizer?.GetValueOrDefault("UserId").ToString()
                    };
                }
            }
            */
        }
    }

    /// <summary>
    /// Example 2: Refactoring ServiceFactory.cs
    /// </summary>
    public static class ServiceFactoryRefactoring
    {
        // BEFORE: Original ServiceFactory.cs
        public static class Before
        {
            /*
            public static class ServiceFactory
            {
                public static ServiceProvider GetServiceProvider()
                {
                    var configuration = new ConfigurationBuilder()
                        .AddSystemsManager($"/{Environment.GetEnvironmentVariable("EnvironmentName")}/Application")
                        .Build();

                    var services = new ServiceCollection();
                    
                    // Configure AWS services
                    services.AddDefaultAWSOptions(configuration.GetAWSOptions());
                    services.AddAWSService<IAmazonDynamoDB>(configuration.GetAWSOptions("DynamoDb"));
                    services.AddAWSService<IAmazonApiGatewayManagementApi>(
                        configuration.GetAWSOptions("ApiGateway")
                    );

                    // Register application options
                    services.AddOptions<ApplicationOptions>();

                    // Register DynamoDB services
                    services.AddTransient<IDynamoDBContext, DynamoDBContext>();
                    services.AddSingleton<IDynamoDbService, DynamoDbService>();

                    // Register HttpClient for Dyte API client
                    services.AddHttpClient();

                    // Register Meeting services
                    services.AddSingleton<DyteApiClientFactory>(
                        sp => new DyteApiClientFactory(sp.GetRequiredService<HttpClient>())
                    );
                    services.AddTransient<DyteApiClient>(
                        sp => sp.GetRequiredService<DyteApiClientFactory>().GetClient()
                    );
                    services.AddTransient<IDyteApiClientWrapper, DyteApiClientWrapper>();
                    services.AddSingleton<IMeetingService, DyteMeetingService>();

                    // Register MediatR
                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OnConnectCommand).Assembly));

                    return services.BuildServiceProvider();
                }
            }
            */
        }

        // AFTER: Refactored ServiceFactory.cs using core package
        public static class After
        {
            /*
            using Flyingdarts.Backend.Signalling.OnConnect.CQRS;
            using Flyingdarts.Lambda.Core.Infrastructure;

            namespace Flyingdarts.Backend.Signalling.OnConnect;

            public static class ServiceFactory
            {
                public static ServiceProvider GetServiceProvider()
                {
                    return Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.GetServiceProvider((services, configuration) =>
                    {
                        // Add your specific services here
                        ConfigureMediatR(services, typeof(OnConnectCommand));
                        ConfigureValidation(services, typeof(OnConnectCommandValidator));
                        
                        // Add any additional services specific to this Lambda
                        services.AddSingleton<IMySpecificService, MySpecificService>();
                    });
                }
            }
            */
        }
    }

    /// <summary>
    /// Example 3: Refactoring InnerHandler.cs
    /// </summary>
    public static class InnerHandlerRefactoring
    {
        // BEFORE: Original InnerHandler.cs
        public static class Before
        {
            /*
            namespace Flyingdarts.Backend.Signalling.OnConnect;

            public class InnerHandler
            {
                private IMediator _mediator;

                public InnerHandler(IMediator mediator)
                {
                    _mediator = mediator;
                }

                public async Task<APIGatewayProxyResponse> Handle(SocketMessage<OnConnectCommand> request)
                {
                    return await _mediator.Send(request.Message);
                }
            }
            */
        }

        // AFTER: Using core package handler (no InnerHandler needed)
        public static class After
        {
            /*
            // No InnerHandler.cs needed! Use MediatRLambdaHandler directly:
            
            var handler = new MediatRLambdaHandler<OnConnectCommand>(services.GetRequiredService<IMediator>());
            
            // The handler automatically handles MediatR requests and returns APIGatewayProxyResponse
            */
        }
    }

    /// <summary>
    /// Example 4: Refactoring Response Creation
    /// </summary>
    public static class ResponseRefactoring
    {
        // BEFORE: Manual response creation
        public static class Before
        {
            /*
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(data),
                Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS,GET,POST,PUT,DELETE" },
                    { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
                }
            };
            */
        }

        // AFTER: Using ResponseBuilder
        public static class After
        {
            /*
            using Flyingdarts.Lambda.Core.Infrastructure;

            return ResponseBuilder.SuccessJson(data);
            */
        }
    }

    /// <summary>
    /// Example 5: Refactoring Tests
    /// </summary>
    public static class TestRefactoring
    {
        // BEFORE: Manual test setup
        public static class Before
        {
            /*
            public class OnConnectTests
            {
                private IServiceProvider _serviceProvider;

                public OnConnectTests()
                {
                    var services = new ServiceCollection();
                    
                    // Configure mocks manually
                    services.AddSingleton<Mock<IAmazonDynamoDB>>();
                    services.AddSingleton<Mock<IAmazonApiGatewayManagementApi>>();
                    services.AddSingleton<Mock<IDynamoDbService>>();
                    
                    // Configure MediatR
                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OnConnectCommand).Assembly));
                    
                    _serviceProvider = services.BuildServiceProvider();
                }

                [Fact]
                public async Task Handle_ValidRequest_ReturnsSuccess()
                {
                    // Arrange
                    var handler = new InnerHandler(_serviceProvider.GetRequiredService<IMediator>());
                    var request = new SocketMessage<OnConnectCommand>
                    {
                        Message = new OnConnectCommand
                        {
                            ConnectionId = "test-connection-id",
                            AuthProviderUserId = "test-user-id"
                        }
                    };

                    // Act
                    var response = await handler.Handle(request);

                    // Assert
                    Assert.NotNull(response);
                    Assert.Equal(200, response.StatusCode);
                }
            }
            */
        }

        // AFTER: Using LambdaTestBase
        public static class After
        {
            /*
            using Flyingdarts.Lambda.Core.Testing;
            using Moq;

            public class OnConnectTests : LambdaTestBase<OnConnectCommand, APIGatewayProxyResponse>
            {
                [Fact]
                public async Task Handle_ValidRequest_ReturnsSuccess()
                {
                    // Arrange
                    Setup();
                    var handler = new MediatRLambdaHandler<OnConnectCommand>(
                        ServiceProvider.GetRequiredService<IMediator>()
                    );
                    var command = new OnConnectCommand
                    {
                        ConnectionId = "test-connection-id",
                        AuthProviderUserId = "test-user-id"
                    };

                    // Act
                    var response = await handler.Handle(command);

                    // Assert
                    AssertSuccess(response);
                }

                [Fact]
                public async Task Handle_WebSocketRequest_ReturnsSuccess()
                {
                    // Arrange
                    Setup();
                    var request = CreateMockWebSocketRequest("test-connection-id", "test-user-id");
                    
                    // Act & Assert using the base test utilities
                    // ... test implementation
                }
            }
            */
        }
    }
}
