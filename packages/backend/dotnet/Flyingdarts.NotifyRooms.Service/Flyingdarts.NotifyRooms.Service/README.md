# Flying Darts Notify Rooms Service

## Overview

The Flying Darts Notify Rooms Service is a .NET 8 library that provides real-time notification capabilities for the Flying Darts gaming platform. This service enables broadcasting messages to multiple WebSocket connections through AWS API Gateway, facilitating real-time communication between players in darts games.

The service is designed to handle concurrent message delivery to multiple connections with robust error handling, timeout management, and connection state monitoring. It integrates seamlessly with AWS API Gateway WebSocket APIs and provides a clean abstraction for sending notifications to game rooms and player groups.

## Features

- **Real-time Notifications**: Broadcast messages to multiple WebSocket connections
- **AWS API Gateway Integration**: Native integration with AWS API Gateway WebSocket APIs
- **Connection Management**: Automatic handling of disconnected or unavailable connections
- **Timeout Protection**: Built-in timeout mechanisms to prevent hanging operations
- **Error Resilience**: Graceful handling of connection failures and network issues
- **Performance Optimization**: Sequential processing to avoid .NET 8 concurrency issues
- **Comprehensive Logging**: Detailed logging for debugging and monitoring
- **Generic Message Support**: Type-safe message handling with generic constraints
- **Cancellation Support**: Full support for cancellation tokens and timeout management

## Prerequisites

- **.NET 8 SDK**: Required for building and using the library
- **AWS API Gateway**: WebSocket API endpoint configured
- **AWS SDK**: Access to AWS services for API Gateway management
- **WebSocket Connections**: Active WebSocket connections to notify

## Installation

1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd flyingdarts-turbo
   ```

2. **Navigate to the notify rooms service**:
   ```bash
   cd packages/backend/dotnet/Flyingdarts.NotifyRooms.Service/Flyingdarts.NotifyRooms.Service
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Build the project**:
   ```bash
   dotnet build
   ```

## Usage

### Basic Setup

1. **Add the service to your DI container**:
   ```csharp
   services.AddScoped<INotifyRoomsService, NotifyRoomsService>();
   services.AddScoped<IAmazonApiGatewayManagementApi, AmazonApiGatewayManagementApiClient>();
   ```

2. **Configure AWS API Gateway client**:
   ```csharp
   services.AddScoped<IAmazonApiGatewayManagementApi>(provider =>
   {
       var config = new AmazonApiGatewayManagementApiConfig
       {
           ServiceURL = "https://your-api-gateway-url.amazonaws.com"
       };
       return new AmazonApiGatewayManagementApiClient(config);
   });
   ```

### Sending Notifications

```csharp
public class GameNotificationService
{
    private readonly INotifyRoomsService _notifyService;

    public GameNotificationService(INotifyRoomsService notifyService)
    {
        _notifyService = notifyService;
    }

    public async Task NotifyGameUpdate(string gameId, string[] connectionIds, GameUpdate update)
    {
        var message = new SocketMessage<GameUpdate>
        {
            Action = "GAME_UPDATE",
            Message = update,
            ConnectionId = "system" // Exclude sender from recipients
        };

        await _notifyService.NotifyRoomAsync(
            message, 
            connectionIds, 
            CancellationToken.None
        );
    }
}
```

### Working with Different Message Types

```csharp
// Game state updates
var gameStateMessage = new SocketMessage<GameState>
{
    Action = "GAME_STATE_CHANGED",
    Message = currentGameState,
    Metadata = new Dictionary<string, object>
    {
        ["gameId"] = gameId,
        ["timestamp"] = DateTime.UtcNow
    }
};

// Player actions
var playerActionMessage = new SocketMessage<PlayerAction>
{
    Action = "PLAYER_ACTION",
    Message = playerAction
};

// Chat messages
var chatMessage = new SocketMessage<ChatMessage>
{
    Action = "CHAT_MESSAGE",
    Message = chatData
};
```

## API Reference

### Core Interfaces

#### INotifyRoomsService
Main service interface for room notifications.

```csharp
public interface INotifyRoomsService
{
    Task NotifyRoomAsync<TNotification>(
        SocketMessage<TNotification> request,
        string[] connectionIds,
        CancellationToken cancellationToken
    );
}
```

### Service Implementation

#### NotifyRoomsService
Main service implementation for sending notifications.

```csharp
public class NotifyRoomsService : INotifyRoomsService
{
    private readonly IAmazonApiGatewayManagementApi _apiGatewayClient;

    public NotifyRoomsService(IAmazonApiGatewayManagementApi apiGatewayClient);

    public async Task NotifyRoomAsync<TNotification>(
        SocketMessage<TNotification> request,
        string[] connectionIds,
        CancellationToken cancellationToken
    );
}
```

### Data Models

#### SocketMessage<TMessage>
Generic message container for WebSocket communications.

```csharp
public class SocketMessage<TMessage>
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("message")]
    public TMessage? Message { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    [JsonIgnore]
    public string? ConnectionId { get; set; }

    public override string ToString();
}
```

### Service Methods

#### NotifyRoomAsync<TNotification>(SocketMessage<TNotification> request, string[] connectionIds, CancellationToken cancellationToken)
Sends a notification to multiple WebSocket connections.

- **Parameters**:
  - `request` (SocketMessage<TNotification>): The message to send
  - `connectionIds` (string[]): Array of connection IDs to notify
  - `cancellationToken` (CancellationToken): Cancellation token for the operation
- **Returns**: Task - Completes when all notifications are processed
- **Throws**: 
  - OperationCanceledException if operation is cancelled
  - Exception for other errors during processing

## Configuration

### AWS API Gateway Configuration
The service requires proper AWS API Gateway WebSocket API configuration:

```json
{
  "AWS": {
    "ApiGateway": {
      "WebSocketEndpoint": "https://your-api-gateway-url.amazonaws.com",
      "Region": "eu-west-1"
    }
  }
}
```

### Environment Variables
The service uses the following environment variables:

- **AWS_REGION**: AWS region for API Gateway
- **AWS_ACCESS_KEY_ID**: AWS access key (if not using IAM roles)
- **AWS_SECRET_ACCESS_KEY**: AWS secret key (if not using IAM roles)

### Service Configuration
Configure the service in your DI container:

```csharp
services.AddScoped<IAmazonApiGatewayManagementApi>(provider =>
{
    var config = new AmazonApiGatewayManagementApiConfig
    {
        ServiceURL = configuration["AWS:ApiGateway:WebSocketEndpoint"],
        RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(
            configuration["AWS:ApiGateway:Region"]
        )
    };
    return new AmazonApiGatewayManagementApiClient(config);
});
```

## Development

### Project Structure
```
Flyingdarts.NotifyRooms.Service/
├── Interfaces/                     # Service interfaces
│   └── INotifyRoomsService.cs     # Main service interface
├── Services/                      # Service implementations
│   └── NotifyRoomsService.cs      # Main service implementation
├── Flyingdarts.NotifyRooms.Service.csproj
└── package.json
```

### Architecture Patterns
- **Service Pattern**: Clean service abstraction for notifications
- **Dependency Injection**: Service lifecycle management
- **Generic Pattern**: Type-safe message handling
- **Error Handling**: Comprehensive error management and recovery
- **Timeout Pattern**: Built-in timeout protection

### Key Features

#### Connection Management
- **Automatic Filtering**: Excludes sender from recipients
- **Connection Validation**: Handles disconnected connections gracefully
- **Sequential Processing**: Avoids .NET 8 concurrency issues
- **Error Isolation**: Individual connection failures don't affect others

#### Performance Optimization
- **Message Serialization**: Single serialization per notification batch
- **Memory Stream Management**: Proper disposal of resources
- **Timeout Protection**: 20-second timeout to prevent hanging
- **Cancellation Support**: Full cancellation token support

#### Error Handling
- **GoneException**: Handles disconnected connections
- **Timeout Handling**: Automatic timeout detection and handling
- **Operation Cancellation**: Proper cancellation token support
- **Logging**: Comprehensive logging for debugging

### Testing
Run unit tests to ensure code quality:
```bash
dotnet test
```

### Code Quality
- Follow C# coding conventions
- Use XML documentation for public APIs
- Implement proper error handling
- Add comprehensive logging
- Use nullable reference types

## Dependencies

### Internal Dependencies
- **Flyingdarts.Core**: Core models and shared functionality

### External Dependencies
- **AWSSDK.ApiGatewayManagementApi** (4.0.0.14): AWS API Gateway management
- **AWSSDK.Core** (4.0.0.16): AWS SDK core functionality
- **AWSSDK.SimpleSystemsManager** (4.0.2.4): AWS Systems Manager integration
- **Microsoft.Extensions.Configuration** (10.0.0-preview.6.25358.103): Configuration management

## Related Projects

### Backend Services
- **[Friends API](../../../apps/backend/dotnet/friends/)**: Friend management and social features
- **[Games API](../../../apps/backend/dotnet/games/)**: Game management and scoring
- **[Signalling API](../../../apps/backend/dotnet/signalling/)**: Real-time communication
- **[Auth API](../../../apps/backend/dotnet/auth/)**: Authentication and authorization

### Frontend Applications
- **[Flutter Mobile App](../../../../frontend/flutter/flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../frontend/angular/fd-app/)**: Web application

### Shared Packages
- **[Core Package](../../../Flyingdarts.Core/)**: Shared business logic and models
- **[Persistence Package](../../../Flyingdarts.Persistence/)**: Data access layer
- **[DynamoDB Service](../../../Flyingdarts.DynamoDb.Service/)**: Database operations

## Version History

- **v0.0.1**: Initial implementation

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Test with real WebSocket connections
6. Maintain error handling patterns

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.