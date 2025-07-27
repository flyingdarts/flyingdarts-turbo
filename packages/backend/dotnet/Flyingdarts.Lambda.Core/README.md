# Flyingdarts.Lambda.Core

Core infrastructure and utilities for Flyingdarts Lambda functions. This package provides reusable components to reduce code duplication and simplify testing across all Lambda functions.

## Features

- **Standardized Lambda Bootstrap**: Common Lambda function setup and execution patterns
- **Base Service Factory**: Shared DI container configuration with common services
- **MediatR Integration**: Base handlers for MediatR-based request processing
- **Response Building**: Utility classes for standardized API Gateway responses
- **Testing Support**: Base test classes and utilities for Lambda function testing

## Usage

### 1. Add the Package Reference

Add the following to your Lambda function's `.csproj` file:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\..\..\..\packages\backend\dotnet\Flyingdarts.Lambda.Core\Flyingdarts.Lambda.Core.csproj" />
</ItemGroup>
```

### 2. Refactor Your Function.cs

Instead of the current pattern:

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

Use the new pattern:

```csharp
using Flyingdarts.Lambda.Core.Infrastructure;
using Flyingdarts.Lambda.Core.Handlers;

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

### 3. Simplify Your ServiceFactory.cs

Instead of duplicating service configuration:

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
        // ... more configuration
        
        return services.BuildServiceProvider();
    }
}
```

Use the base service factory:

```csharp
using Flyingdarts.Lambda.Core.Infrastructure;

public static class ServiceFactory
{
    public static ServiceProvider GetServiceProvider()
    {
        return Flyingdarts.Lambda.Core.Infrastructure.ServiceFactory.GetServiceProvider((services, configuration) =>
        {
            // Add your specific services here
            ConfigureMediatR(services, typeof(OnConnectCommand));
            ConfigureValidation(services, typeof(OnConnectCommandValidator));
        });
    }
}
```

### 4. Use ResponseBuilder for Standardized Responses

Instead of manually creating responses:

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

Use the ResponseBuilder:

```csharp
using Flyingdarts.Lambda.Core.Infrastructure;

return ResponseBuilder.SuccessJson(data);
```

### 5. Simplify Testing

Create test classes that inherit from the base test class:

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
}
```

## Benefits

1. **Reduced Code Duplication**: Common patterns are centralized in the core package
2. **Easier Testing**: Base test classes provide common testing utilities
3. **Consistent Responses**: Standardized response building across all functions
4. **Simplified Setup**: Common service configuration is handled automatically
5. **Better Maintainability**: Changes to common patterns only need to be made in one place

## Migration Guide

To migrate existing Lambda functions:

1. Add the package reference to your `.csproj`
2. Replace your `Function.cs` with the new bootstrap pattern
3. Simplify your `ServiceFactory.cs` to use the base factory
4. Update your tests to inherit from `LambdaTestBase`
5. Replace manual response creation with `ResponseBuilder`

## Contributing

When adding new common functionality:

1. Add it to the appropriate namespace in the core package
2. Update this README with usage examples
3. Ensure all existing Lambda functions are updated to use the new functionality 