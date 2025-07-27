# Flyingdarts.Core

## Overview

The Flyingdarts.Core package provides essential core functionality and shared components for the Flying Darts Turbo platform. This package contains fundamental models, extensions, and utilities that are used across multiple services in the backend infrastructure.

The package is designed to:
- Provide shared data models for WebSocket communication
- Offer extension methods for AWS Lambda API Gateway integration
- Support JSON serialization and deserialization
- Enable consistent message handling across services
- Provide reusable utilities for AWS Lambda functions

## Features

- **WebSocket Message Models**: Generic `SocketMessage<T>` class for structured WebSocket communication
- **API Gateway Extensions**: Extension methods for converting API Gateway requests to socket messages
- **JSON Serialization**: Built-in JSON serialization support with System.Text.Json
- **AWS Lambda Integration**: Native support for AWS Lambda and API Gateway events
- **Type Safety**: Strongly-typed message handling with generic support
- **Error Handling**: Robust error handling for malformed requests
- **Performance Optimized**: Uses ReadOnlySpan and MemoryStream for efficient processing

## Prerequisites

- .NET 8.0 or later
- AWS Lambda Core package
- Amazon.Lambda.APIGatewayEvents package
- Microsoft.Extensions.Configuration

## Installation

Add the package to your project file:

```xml
<ItemGroup>
  <ProjectReference Include="../../packages/backend/dotnet/Flyingdarts.Core/Flyingdarts.Core.csproj" />
</ItemGroup>
```

Or reference it in your solution:

```xml
<ProjectReference Include="packages/backend/dotnet/Flyingdarts.Core/Flyingdarts.Core.csproj" />
```

## Usage

### WebSocket Message Handling

```csharp
using Flyingdarts.Core.Models;

// Create a socket message with custom payload
var message = new SocketMessage<string>
{
    Action = "user_message",
    Message = "Hello, World!",
    Metadata = new Dictionary<string, object>
    {
        ["timestamp"] = DateTime.UtcNow,
        ["user_id"] = "user123"
    }
};

// Serialize to JSON
string json = message.ToString();
```

### API Gateway Request Conversion

```csharp
using Flyingdarts.Core.Extensions;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Convert API Gateway request to socket message
public async Task<APIGatewayProxyResponse> FunctionHandler(
    APIGatewayProxyRequest request, 
    ILambdaContext context)
{
    var serializer = new Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer();
    
    // Convert request to typed socket message
    var socketMessage = request.To<MyMessageType>(serializer);
    
    if (socketMessage != null)
    {
        // Process the message
        var connectionId = socketMessage.ConnectionId;
        var action = socketMessage.Action;
        var payload = socketMessage.Message;
        
        // Your business logic here
    }
    
    return new APIGatewayProxyResponse
    {
        StatusCode = 200,
        Body = "Message processed"
    };
}
```

### Custom Message Types

```csharp
// Define your custom message type
public class ChatMessage
{
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

// Use with SocketMessage
var chatMessage = new SocketMessage<ChatMessage>
{
    Action = "chat_message",
    Message = new ChatMessage
    {
        UserId = "user123",
        Content = "Hello everyone!",
        Timestamp = DateTime.UtcNow
    }
};
```

## API Reference

### Models

#### SocketMessage<TMessage>

A generic class for structured WebSocket communication.

**Properties:**
- `Action` (string?): The action identifier for the message
- `Message` (TMessage?): The typed message payload
- `Metadata` (Dictionary<string, object>?): Additional metadata for the message
- `ConnectionId` (string?): The WebSocket connection identifier (JSON ignored)

**Methods:**
- `ToString()`: Serializes the message to JSON string

### Extensions

#### APIGatewayProxyRequestExtensions

Extension methods for API Gateway request processing.

**Methods:**

##### To<T>(APIGatewayProxyRequest, ILambdaSerializer): SocketMessage<T>?

Converts an API Gateway proxy request to a typed SocketMessage.

**Parameters:**
- `request` (APIGatewayProxyRequest): The API Gateway proxy request
- `serializer` (ILambdaSerializer): The Lambda serializer for deserialization

**Returns:**
- `SocketMessage<T>?`: The converted socket message or null if conversion fails

**Exceptions:**
- `ArgumentNullException`: Thrown when request or serializer is null

## Configuration

The package uses the following configuration options:

- **Target Framework**: .NET 8.0
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled
- **JSON Serialization**: System.Text.Json
- **Optimizations**: Various .NET 8 performance optimizations enabled

## Development

### Building the Package

```bash
dotnet build Flyingdarts.Core.csproj
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

## Dependencies

### Internal Dependencies
- None (this is a core package)

### External Dependencies
- **Amazon.Lambda.APIGatewayEvents** (2.7.1): AWS Lambda API Gateway event models
- **Amazon.Lambda.Core** (2.7.0): AWS Lambda core functionality
- **AWSSDK.SimpleSystemsManagement** (4.0.2.4): AWS Systems Manager integration
- **Microsoft.Extensions.Configuration** (10.0.0-preview.6.25358.103): Configuration management

## Related Projects

- **Flyingdarts.Lambda.Core**: Lambda function core utilities
- **Flyingdarts.Connection.Services**: WebSocket connection management
- **Flyingdarts.Meetings.Service**: Meeting management services
- **Flyingdarts.NotifyRooms.Service**: Room notification services

## Contributing

When contributing to this package:

1. Follow the existing code style and patterns
2. Add XML documentation for all public APIs
3. Include unit tests for new functionality
4. Ensure nullable reference types are properly handled
5. Update this README for any new features or breaking changes

## License

This package is part of the Flying Darts Turbo monorepo and follows the same licensing terms.