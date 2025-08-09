# Flyingdarts.Backend.Signalling.Api

## Overview

The Flyingdarts.Backend.Signalling.Api service is a .NET 8 AWS Lambda function that provides real-time WebSocket-based signalling for the Flyingdarts Turbo platform. This service handles WebSocket connections, manages real-time communication between clients, and coordinates peer-to-peer connections for features like voice chat and game synchronization.

This service is responsible for:
- Managing WebSocket connections for real-time communication
- Handling signalling for WebRTC peer-to-peer connections
- Coordinating room-based communication for multiplayer games
- Managing connection state and presence information
- Supporting real-time messaging between connected clients
- Integrating with AWS API Gateway for WebSocket management

## Features

- **Real-time WebSocket Communication**: Handles live messaging and signalling
- **WebRTC Signalling**: Supports peer-to-peer connection establishment
- **Room Management**: Manages communication rooms for multiplayer games
- **Connection State Management**: Tracks connection status and presence
- **AWS Lambda Integration**: Serverless deployment with automatic scaling
- **MediatR Pattern**: Clean separation of concerns with command/query handlers
- **Dependency Injection**: Flexible service composition and testing
- **Validation**: Request validation using FluentValidation
- **Error Handling**: Comprehensive error handling with proper WebSocket responses
- **JWT Token Validation**: Secure authentication for WebSocket connections

## Prerequisites

- .NET 8 SDK
- AWS CLI configured with appropriate permissions
- DynamoDB tables configured for connection data
- Visual Studio 2022 or VS Code with C# extensions
- Docker (for containerized deployment)

## Installation

1. Clone the monorepo and navigate to the signalling API:
```bash
cd apps/backend/dotnet/signalling/api
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run tests:
```bash
dotnet test
```

## Usage

### Local Development

Run the signalling API locally for development:

```bash
dotnet run
```

### AWS Lambda Deployment

1. Build for AWS Lambda:
```bash
dotnet lambda package
```

2. Deploy using AWS CLI or CDK:
```bash
aws lambda update-function-code --function-name flyingdarts-signalling-api --zip-file fileb://Flyingdarts.Backend.Signalling.Api.zip
```

### WebSocket Endpoints

The API exposes the following WebSocket endpoints:

#### Connection Management

##### Connect
- **Route**: `$connect`
- **Method**: WebSocket connection
- **Description**: Establishes a new WebSocket connection

##### Disconnect
- **Route**: `$disconnect`
- **Method**: WebSocket disconnection
- **Description**: Handles WebSocket connection termination

#### Signalling Operations

##### Join Room
- **Route**: `signalling/join`
- **Method**: WebSocket message
- **Description**: Join a signalling room for peer-to-peer communication

##### Leave Room
- **Route**: `signalling/leave`
- **Method**: WebSocket message
- **Description**: Leave a signalling room

##### Send Message
- **Route**: `signalling/message`
- **Method**: WebSocket message
- **Description**: Send a message to other clients in the room

##### Offer/Answer Exchange
- **Route**: `signalling/offer`, `signalling/answer`
- **Method**: WebSocket message
- **Description**: Exchange WebRTC offer/answer for peer connection

##### ICE Candidate Exchange
- **Route**: `signalling/ice-candidate`
- **Method**: WebSocket message
- **Description**: Exchange ICE candidates for WebRTC connection

## API Reference

### Main Classes

#### `Function`

The main Lambda function entry point that handles API Gateway WebSocket requests.

**Properties:**
- `ILogger<Function> _logger`: Logging instance for the function
- `ISignallingApiBootstrap _bootstrap`: Bootstrap instance for request handling

**Methods:**

##### `FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context): Task<APIGatewayProxyResponse>`

Main handler for API Gateway WebSocket requests.

**Parameters:**
- `request` (APIGatewayProxyRequest): The WebSocket request from API Gateway
- `context` (ILambdaContext): Lambda execution context

**Returns:**
- `Task<APIGatewayProxyResponse>`: WebSocket response with appropriate status code

**Throws:**
- `ArgumentException`: When request is malformed
- `UnauthorizedAccessException`: When user is not authenticated

#### `SignallingApiBootstrap`

The main bootstrap class that handles WebSocket request routing and processing.

**Properties:**
- `IMediator _mediator`: MediatR mediator for handling commands and queries
- `ILogger<SignallingApiBootstrap> _logger`: Logging instance

**Methods:**

##### `HandleRequestAsync(APIGatewayProxyRequest request): Task<APIGatewayProxyResponse>`

Handles incoming WebSocket requests and routes them to appropriate handlers.

**Parameters:**
- `request` (APIGatewayProxyRequest): The incoming WebSocket request

**Returns:**
- `Task<APIGatewayProxyResponse>`: Processed response

##### `ConvertRequest(APIGatewayProxyRequest request): IRequest<APIGatewayProxyResponse>`

Converts incoming API Gateway requests to appropriate MediatR commands.

**Parameters:**
- `request` (APIGatewayProxyRequest): The incoming API Gateway request

**Returns:**
- `IRequest<APIGatewayProxyResponse>`: MediatR request object

#### `ServiceFactory`

Factory class for setting up dependency injection and service configuration.

**Methods:**

##### `CreateServiceProvider(): IServiceProvider`

Creates and configures the dependency injection container.

**Returns:**
- `IServiceProvider`: Configured service provider

### Request Models

#### Connection Models

##### `ConnectCommand`

Command for handling WebSocket connection establishment.

**Properties:**
- `string ConnectionId` (get; set;): Unique WebSocket connection identifier
- `string UserId` (get; set;): User identifier from JWT token
- `Dictionary<string, string> QueryStringParameters` (get; set;): Connection parameters

##### `DisconnectCommand`

Command for handling WebSocket connection termination.

**Properties:**
- `string ConnectionId` (get; set;): WebSocket connection identifier to disconnect

#### Signalling Models

##### `JoinRoomCommand`

Command for joining a signalling room.

**Properties:**
- `string ConnectionId` (get; set;): WebSocket connection identifier
- `string UserId` (get; set;): User identifier
- `string RoomId` (get; set;): Room identifier to join

##### `LeaveRoomCommand`

Command for leaving a signalling room.

**Properties:**
- `string ConnectionId` (get; set;): WebSocket connection identifier
- `string RoomId` (get; set;): Room identifier to leave

##### `SendMessageCommand`

Command for sending a message to room participants.

**Properties:**
- `string ConnectionId` (get; set;): Sender's connection identifier
- `string RoomId` (get; set;): Target room identifier
- `string Message` (get; set;): Message content to send
- `string MessageType` (get; set;): Type of message (offer, answer, ice-candidate, etc.)

### Response Models

#### `SignallingResponse`

Base response model for signalling operations.

**Properties:**
- `bool Success` (get; set;): Whether the operation was successful
- `string Message` (get; set;): Response message
- `string ConnectionId` (get; set;): Related connection identifier

#### `RoomInfo`

Information about a signalling room.

**Properties:**
- `string RoomId` (get; set;): Room identifier
- `List<string> ParticipantIds` (get; set;): List of participant connection IDs
- `int ParticipantCount` (get; set;): Number of participants in the room
- `DateTime CreatedAt` (get; set;): When the room was created

## Configuration

### Environment Variables

- `AWS_REGION`: AWS region for service deployment
- `DYNAMODB_TABLE`: DynamoDB table for connection data storage
- `CONNECTION_TABLE`: DynamoDB table for WebSocket connections
- `LOG_LEVEL`: Logging level (default: Information)

### AWS Systems Manager Parameters

- `/flyingdarts/signalling/dynamodb-table`: DynamoDB table name
- `/flyingdarts/signalling/connection-table`: Connection table name
- `/flyingdarts/signalling/region`: AWS region
- `/flyingdarts/signalling/log-level`: Logging level

### Dependencies

**Internal Dependencies:**
- `Flyingdarts.Lambda.Core`: Core Lambda infrastructure
- `Flyingdarts.DynamoDb.Service`: DynamoDB data access
- `Flyingdarts.Meetings.Service`: Meeting and room management
- `Flyingdarts.Persistence`: Data persistence layer

**External Dependencies:**
- `Amazon.Lambda.APIGatewayEvents`: API Gateway event types
- `Amazon.Lambda.RuntimeSupport`: Lambda runtime support
- `Amazon.Lambda.Serialization.SystemTextJson`: JSON serialization
- `Amazon.Extensions.Configuration.SystemsManager`: AWS Systems Manager
- `MediatR`: Mediator pattern implementation
- `Microsoft.Extensions.Configuration`: Configuration management
- `Microsoft.Extensions.DependencyInjection`: Dependency injection
- `Microsoft.Extensions.Http`: HTTP client factory
- `AWSSDK.ApiGatewayManagementApi`: API Gateway Management API
- `AWSSDK.DynamoDBv2`: DynamoDB SDK
- `FluentValidation`: Request validation
- `System.IdentityModel.Tokens.Jwt`: JWT validation

## Development

### Project Structure

```
signalling/
├── api/                           # Main API project
│   ├── Function.cs                # Lambda entry point
│   ├── SignallingApiBootstrap.cs  # Main bootstrap class
│   ├── ServiceFactory.cs          # Dependency injection setup
│   ├── Requests/                  # Request handlers
│   │   ├── Connect/               # Connection requests
│   │   ├── Disconnect/            # Disconnection requests
│   │   └── Signalling/            # Signalling requests
│   ├── Flyingdarts.Backend.Signalling.Api.csproj  # Project file
│   └── README.md                  # This documentation
└── tests/                         # Test project
    └── Flyingdarts.Backend.Signalling.Api.Tests.csproj
```

### Testing

Run the test suite:

```bash
dotnet test
```

Run with verbose output:

```bash
dotnet test --verbosity normal
```

### Building

Build for development:

```bash
dotnet build
```

Build for production:

```bash
dotnet build --configuration Release
```

Build for AWS Lambda:

```bash
dotnet lambda package
```

### Code Quality

The project uses:
- **StyleCop**: Code style enforcement
- **FluentValidation**: Request validation
- **MediatR**: Clean architecture patterns
- **Dependency Injection**: Service composition

Run quality checks:

```bash
dotnet build --verbosity normal
```

## WebRTC Signalling Protocol

### Connection Flow

1. **Client Connection**: Client establishes WebSocket connection
2. **Authentication**: JWT token validation for secure access
3. **Room Join**: Client joins a specific signalling room
4. **Peer Discovery**: Client discovers other peers in the room
5. **Offer/Answer Exchange**: WebRTC offer/answer exchange
6. **ICE Candidate Exchange**: ICE candidate exchange for connection
7. **Peer Connection**: Direct peer-to-peer connection established

### Message Types

#### Connection Messages
- `connect`: Establish WebSocket connection
- `disconnect`: Terminate WebSocket connection

#### Room Messages
- `join-room`: Join a signalling room
- `leave-room`: Leave a signalling room
- `room-info`: Get room participant information

#### WebRTC Messages
- `offer`: WebRTC offer for peer connection
- `answer`: WebRTC answer for peer connection
- `ice-candidate`: ICE candidate for connection establishment
- `ice-candidate-remove`: Remove ICE candidate

#### Game Messages
- `game-state`: Game state synchronization
- `player-action`: Player action broadcast
- `game-event`: Game event notification

## Security Considerations

### Authentication

- **JWT Validation**: All WebSocket connections require valid JWT tokens
- **Connection Authorization**: Verify user permissions for room access
- **Message Validation**: Validate all incoming message content

### Data Privacy

- **Room Isolation**: Messages are only sent to room participants
- **Connection Privacy**: Connection IDs are not exposed to unauthorized users
- **Message Encryption**: Consider end-to-end encryption for sensitive data

## Performance Considerations

### WebSocket Management

- **Connection Limits**: Monitor concurrent WebSocket connections
- **Message Throughput**: Optimize for high-frequency messaging
- **Memory Usage**: Efficient connection state management
- **Timeout Handling**: Proper handling of stale connections

### DynamoDB Optimization

- **Connection Storage**: Efficient storage of connection metadata
- **Room Queries**: Optimized queries for room participant lists
- **TTL Management**: Automatic cleanup of expired connections

## Monitoring and Logging

### CloudWatch Logs

Monitor signalling operations:

```bash
aws logs filter-log-events --log-group-name /aws/lambda/flyingdarts-signalling-api
```

### Key Metrics

- WebSocket connection success/failure rates
- Message delivery latency
- Room participant counts
- WebRTC connection establishment success rates

### Alerts

Set up CloudWatch alarms for:
- High connection failure rates
- Increased message latency
- DynamoDB throttling
- Unauthorized connection attempts

## Troubleshooting

### Common Issues

1. **Connection Failures**: Check API Gateway WebSocket configuration
2. **Message Delivery**: Verify room membership and connection state
3. **WebRTC Issues**: Check offer/answer exchange flow
4. **Authentication Errors**: Verify JWT token validation

### Debugging

Enable detailed logging:

```bash
export LOG_LEVEL=Debug
```

Check CloudWatch logs for detailed execution information.

## Related Projects

- **flyingdarts-x01-api**: Game API that uses signalling for real-time updates
- **flyingdarts-meetings-service**: Meeting management for room coordination
- **flyingdarts-dynamodb-service**: Data persistence for connection state
- **flyingdarts-lambda-core**: Core Lambda infrastructure

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow MediatR patterns for new commands/queries

## License

Part of the Flyingdarts Turbo monorepo. See root LICENSE file for details.
