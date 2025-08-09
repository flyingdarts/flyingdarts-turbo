# Flyingdarts.Connection.Services

## Overview

The Flyingdarts.Connection.Services package provides WebSocket connection management functionality for the Flyingdarts Turbo platform. This package handles the association between player identities and their WebSocket connection IDs, enabling real-time communication between players and the game server.

The package is designed to:
- Manage WebSocket connection mappings for players
- Update player connection IDs in the database
- Provide a clean interface for connection management
- Integrate with DynamoDB for persistent storage
- Support async operations with cancellation token support

## Features

- **Connection Management**: Update and track player WebSocket connection IDs
- **DynamoDB Integration**: Persistent storage of connection mappings
- **Async Operations**: Full async/await support with cancellation tokens
- **Error Handling**: Comprehensive validation and error handling
- **Dependency Injection**: Clean interface design for easy testing
- **Type Safety**: Strongly-typed operations with nullable reference types
- **Performance Optimized**: Efficient database operations with proper indexing

## Prerequisites

- .NET 8.0 or later
- AWS DynamoDB service
- Flyingdarts.DynamoDb.Service package
- Flyingdarts.Persistence package
- Microsoft.Extensions.Configuration

## Installation

Add the package to your project file:

```xml
<ItemGroup>
  <ProjectReference Include="../../packages/backend/dotnet/Flyingdarts.Connection.Services/Flyingdarts.Connection.Services.csproj" />
</ItemGroup>
```

Or reference it in your solution:

```xml
<ProjectReference Include="packages/backend/dotnet/Flyingdarts.Connection.Services/Flyingdarts.Connection.Services.csproj" />
```

## Usage

### Basic Connection Management

```csharp
using Flyingdarts.Connection.Services;

// Register the service in your DI container
services.AddScoped<IConnectionService, ConnectionService>();

// Use the service in your Lambda function or controller
public class WebSocketHandler
{
    private readonly IConnectionService _connectionService;
    
    public WebSocketHandler(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    
    public async Task HandleConnection(string playerId, string connectionId)
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        await _connectionService.UpdateConnectionIdAsync(
            playerId, 
            connectionId, 
            cancellationTokenSource.Token
        );
    }
}
```

### Lambda Function Integration

```csharp
using Flyingdarts.Connection.Services;
using Amazon.Lambda.APIGatewayEvents;

public class OnConnectHandler
{
    private readonly IConnectionService _connectionService;
    
    public OnConnectHandler(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
    {
        var connectionId = request.RequestContext.ConnectionId;
        var playerId = request.RequestContext.Authorizer?["UserId"]?.ToString();
        
        if (string.IsNullOrEmpty(playerId))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 401,
                Body = "Unauthorized"
            };
        }
        
        try
        {
            await _connectionService.UpdateConnectionIdAsync(
                playerId, 
                connectionId, 
                CancellationToken.None
            );
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "Connected successfully"
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = $"Connection failed: {ex.Message}"
            };
        }
    }
}
```

### Error Handling

```csharp
public async Task HandleConnectionWithErrorHandling(string playerId, string connectionId)
{
    try
    {
        await _connectionService.UpdateConnectionIdAsync(
            playerId, 
            connectionId, 
            CancellationToken.None
        );
        
        Console.WriteLine($"Successfully updated connection for player {playerId}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Invalid parameters: {ex.Message}");
        // Handle invalid input
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database operation failed: {ex.Message}");
        // Handle database errors
    }
}
```

## API Reference

### Interfaces

#### IConnectionService

The main interface for connection management operations.

**Methods:**

##### UpdateConnectionIdAsync(string, string, CancellationToken): Task

Updates the connection ID for a specific player.

**Parameters:**
- `playerId` (string): The unique identifier of the player
- `connectionId` (string): The WebSocket connection identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task`: Async operation that completes when the connection is updated

**Exceptions:**
- `ArgumentException`: Thrown when playerId or connectionId is null or empty
- `Exception`: Thrown when database operations fail

### Classes

#### ConnectionService

The concrete implementation of the connection service.

**Properties:**
- `DynamoDbService` (IDynamoDbService): The DynamoDB service for data persistence

**Constructor:**
- `ConnectionService(IDynamoDbService)`: Creates a new connection service instance

**Methods:**
- `UpdateConnectionIdAsync(string, string, CancellationToken)`: Updates player connection ID

## Configuration

The package uses the following configuration options:

- **Target Framework**: .NET 8.0
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled
- **Language Version**: Latest
- **Optimizations**: Various .NET 8 performance optimizations enabled

### Dependency Injection Setup

```csharp
// In your Startup.cs or Program.cs
services.AddScoped<IConnectionService, ConnectionService>();

// Ensure DynamoDB service is also registered
services.AddScoped<IDynamoDbService, DynamoDbService>();
```

## Development

### Building the Package

```bash
dotnet build Flyingdarts.Connection.Services.csproj
```

### Running Tests

```bash
dotnet test
```

### Code Style

The project follows .NET coding conventions with:
- XML documentation for public APIs
- Nullable reference types enabled
- Implicit usings for cleaner code
- Consistent naming conventions
- Proper error handling and validation

## Dependencies

### Internal Dependencies
- **Flyingdarts.DynamoDb.Service**: DynamoDB service for data persistence
- **Flyingdarts.Persistence**: Data persistence models and utilities

### External Dependencies
- **AWSSDK.SimpleSystemsManagement** (4.0.2.4): AWS Systems Manager integration
- **Microsoft.Extensions.Configuration** (10.0.0-preview.6.25358.103): Configuration management

## Database Schema

The connection service works with the following DynamoDB table structure:

### Users Table
- **Primary Key**: `PlayerId` (string)
- **Attributes**:
  - `ConnectionId` (string): Current WebSocket connection ID
  - `LastConnected` (string): ISO 8601 timestamp of last connection
  - Additional user attributes as defined in the persistence layer

## Related Projects

- **Flyingdarts.DynamoDb.Service**: DynamoDB service implementation
- **Flyingdarts.Persistence**: Data persistence models
- **Flyingdarts.Core**: Core models and utilities
- **Flyingdarts.Lambda.Core**: Lambda function infrastructure

## Contributing

When contributing to this package:

1. Follow the existing code style and patterns
2. Add XML documentation for all public APIs
3. Include unit tests for new functionality
4. Ensure proper error handling and validation
5. Update this README for any new features or breaking changes
6. Test with real DynamoDB integration

## License

This package is part of the Flyingdarts Turbo monorepo and follows the same licensing terms.
