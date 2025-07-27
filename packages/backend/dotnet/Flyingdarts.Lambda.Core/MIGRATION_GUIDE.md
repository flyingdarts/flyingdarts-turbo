# Migration Guide: Converting Lambda Functions to Use Flyingdarts.Lambda.Core

This guide provides step-by-step instructions for migrating existing Lambda functions to use the new `Flyingdarts.Lambda.Core` package.

## Prerequisites

1. Ensure the `Flyingdarts.Lambda.Core` package is built and available
2. Have access to the Lambda function source code you want to migrate

## Step 1: Add Package Reference

Add the following to your Lambda function's `.csproj` file:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\..\..\..\packages\backend\dotnet\Flyingdarts.Lambda.Core\Flyingdarts.Lambda.Core.csproj" />
</ItemGroup>
```

## Step 2: Refactor Function.cs

### Before:
```csharp
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
```

### After:
```csharp
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
```

## Step 3: Simplify ServiceFactory.cs

### Before:
```csharp
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
```

### After:
```csharp
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
```

## Step 4: Remove InnerHandler.cs (Optional)

If your Lambda function has an `InnerHandler.cs` file that simply wraps MediatR, you can remove it entirely and use `MediatRLambdaHandler<T>` directly.

### Before:
```csharp
// InnerHandler.cs
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
```

### After:
```csharp
// No InnerHandler.cs needed! Use MediatRLambdaHandler directly:
var handler = new MediatRLambdaHandler<OnConnectCommand>(services.GetRequiredService<IMediator>());
```

## Step 5: Update Response Creation

Replace manual response creation with `ResponseBuilder`:

### Before:
```csharp
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
```

### After:
```csharp
using Flyingdarts.Lambda.Core.Infrastructure;

return ResponseBuilder.SuccessJson(data);
```

## Step 6: Update Tests

### Before:
```csharp
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
```

### After:
```csharp
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
```

## Step 7: Update GlobalUsings.cs (Optional)

You can remove common using statements from your `GlobalUsings.cs` since they're now provided by the core package:

### Before:
```csharp
global using System;
global using System.Collections.Generic;
global using System.Text.Json;
global using System.Threading;
global using System.Threading.Tasks;
global using Amazon.DynamoDBv2;
global using Amazon.DynamoDBv2.Model;
global using Amazon.Lambda.APIGatewayEvents;
global using Amazon.Lambda.RuntimeSupport;
global using Amazon.Lambda.Serialization.SystemTextJson;
global using MediatR;
global using Microsoft.Extensions.DependencyInjection;
```

### After:
```csharp
// Keep only Lambda-specific using statements
global using Flyingdarts.Backend.Signalling.OnConnect;
global using Flyingdarts.Backend.Signalling.OnConnect.CQRS;
global using Flyingdarts.Core.Models;
```

## Step 8: Build and Test

1. Build the project to ensure all references are resolved
2. Run existing tests to ensure functionality is preserved
3. Deploy and test the Lambda function in your environment

## Troubleshooting

### Common Issues:

1. **Missing Assembly Reference**: Ensure the core package is built before referencing it
2. **Namespace Conflicts**: Use fully qualified names if there are conflicts
3. **Missing Dependencies**: Add any missing dependencies to the core package or your Lambda function

### Getting Help:

- Check the `README.md` for detailed usage examples
- Review the `Examples/RefactoringExamples.cs` file for more patterns
- Ensure all required packages are referenced in your `.csproj` file

## Benefits After Migration

1. **Reduced Code**: Significantly less boilerplate code in each Lambda function
2. **Consistent Patterns**: All Lambda functions follow the same structure
3. **Easier Testing**: Common test utilities and mock setup
4. **Better Maintainability**: Changes to common patterns only need to be made in one place
5. **Standardized Responses**: Consistent API Gateway responses across all functions 