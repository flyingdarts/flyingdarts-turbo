# Flyingdarts X01 Games API

## Overview

The Flyingdarts X01 Games API is a .NET 8 AWS Lambda function that provides real-time WebSocket-based game management for X01 darts games. This service handles game creation, player joining, and score tracking through WebSocket connections, enabling multiplayer darts gameplay in the Flyingdarts Turbo platform.

This API is responsible for:
- Managing X01 game sessions through WebSocket connections
- Handling real-time game state updates and player interactions
- Processing game creation, joining, and scoring requests
- Integrating with AWS API Gateway for WebSocket management
- Coordinating with other backend services for game persistence and notifications

## Features

- **Real-time WebSocket Communication**: Handles live game updates and player interactions
- **X01 Game Logic**: Implements standard X01 darts game rules and scoring
- **Multiplayer Support**: Manages multiple players in concurrent game sessions
- **AWS Lambda Integration**: Serverless deployment with automatic scaling
- **MediatR Pattern**: Clean separation of concerns with command/query handlers
- **Dependency Injection**: Flexible service composition and testing
- **Error Handling**: Comprehensive error handling with proper HTTP responses

## Prerequisites

- .NET 8 SDK
- AWS CLI configured with appropriate permissions
- Visual Studio 2022 or VS Code with C# extensions
- Docker (for containerized deployment)

## Installation

1. Clone the monorepo and navigate to the API:
```bash
cd apps/backend/dotnet/api
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

Run the API locally for development:

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
aws lambda update-function-code --function-name flyingdarts-x01-api --zip-file lambda.zip
```

### WebSocket Endpoints

The API exposes the following WebSocket endpoints:

#### Create Game
- **Route**: `games/x01/create`
- **Method**: WebSocket message
- **Description**: Creates a new X01 game session

#### Join Game
- **Route**: `games/x01/join`
- **Method**: WebSocket message
- **Description**: Allows a player to join an existing game

#### Score Update
- **Route**: `games/x01/score`
- **Method**: WebSocket message
- **Description**: Records a player's score in the current game

## API Reference

### Main Classes

#### `X01ApiBootstrap`

The main bootstrap class that handles AWS Lambda initialization and request routing.

**Properties:**
- `IMediator _mediator`: MediatR mediator for handling commands and queries

**Methods:**

##### `ConvertRequest(APIGatewayProxyRequest request): IRequest<APIGatewayProxyResponse>`

Converts incoming API Gateway requests to appropriate MediatR commands.

**Parameters:**
- `request` (APIGatewayProxyRequest): The incoming API Gateway request

**Returns:**
- `IRequest<APIGatewayProxyResponse>`: MediatR request object

**Throws:**
- `ArgumentException`: When request context is invalid or route key is unknown

##### `ConvertCreateRequest(APIGatewayProxyRequest request, DefaultLambdaJsonSerializer serializer): IRequest<APIGatewayProxyResponse>`

Converts create game requests to CreateX01GameCommand.

**Parameters:**
- `request` (APIGatewayProxyRequest): The incoming request
- `serializer` (DefaultLambdaJsonSerializer): JSON serializer instance

**Returns:**
- `IRequest<APIGatewayProxyResponse>`: CreateX01GameCommand instance

##### `ConvertJoinRequest(APIGatewayProxyRequest request, DefaultLambdaJsonSerializer serializer): IRequest<APIGatewayProxyResponse>`

Converts join game requests to JoinX01GameCommand.

**Parameters:**
- `request` (APIGatewayProxyRequest): The incoming request
- `serializer` (DefaultLambdaJsonSerializer): JSON serializer instance

**Returns:**
- `IRequest<APIGatewayProxyResponse>`: JoinX01GameCommand instance

##### `ConvertScoreRequest(APIGatewayProxyRequest request, DefaultLambdaJsonSerializer serializer): IRequest<APIGatewayProxyResponse>`

Converts score update requests to CreateX01ScoreCommand.

**Parameters:**
- `request` (APIGatewayProxyRequest): The incoming request
- `serializer` (DefaultLambdaJsonSerializer): JSON serializer instance

**Returns:**
- `IRequest<APIGatewayProxyResponse>`: CreateX01ScoreCommand instance

#### `X01ApiWebSocketMethods`

Static class containing WebSocket route constants.

**Constants:**
- `Create` (string): Route for creating new games (`games/x01/create`)
- `Join` (string): Route for joining existing games (`games/x01/join`)
- `Score` (string): Route for updating scores (`games/x01/score`)

#### `IConnectable`

Interface for objects that can establish WebSocket connections.

**Properties:**
- `string ConnectionId` (get; set;): Unique identifier for the WebSocket connection

#### `Connectable`

Base class implementing IConnectable interface.

**Properties:**
- `string ConnectionId` (get; set;): Unique identifier for the WebSocket connection

**Methods:**

##### `IsConnected(): bool`

Checks if the object has a valid connection ID.

**Returns:**
- `bool`: True if ConnectionId is not null or empty

### Request Models

#### CreateX01GameCommand

Command for creating a new X01 game.

**Properties:**
- `string ConnectionId` (get; set;): WebSocket connection identifier
- `string PlayerId` (get; set;): Player creating the game
- Additional game creation parameters

#### JoinX01GameCommand

Command for joining an existing X01 game.

**Properties:**
- `string ConnectionId` (get; set;): WebSocket connection identifier
- `string PlayerId` (get; set;): Player joining the game
- `string GameId` (get; set;): ID of the game to join

#### CreateX01ScoreCommand

Command for recording a score in an X01 game.

**Properties:**
- `string ConnectionId` (get; set;): WebSocket connection identifier
- `string PlayerId` (get; set;): Player recording the score
- `string GameId` (get; set;): ID of the game
- `int Score` (get; set;): Score value to record

## Configuration

### Environment Variables

- `AWS_REGION`: AWS region for service deployment
- `DYNAMODB_TABLE`: DynamoDB table for game data storage
- `CONNECTION_TABLE`: DynamoDB table for WebSocket connections
- `LOG_LEVEL`: Logging level (default: Information)

### Dependencies

**Internal Dependencies:**
- `Flyingdarts.Lambda.Core`: Core Lambda infrastructure
- `Flyingdarts.Connection.Services`: WebSocket connection management
- `Flyingdarts.Core`: Core business logic and extensions
- `Flyingdarts.DynamoDb.Service`: DynamoDB data access
- `Flyingdarts.Meetings.Service`: Meeting and room management
- `Flyingdarts.Metadata.Services`: Metadata management
- `Flyingdarts.NotifyRooms.Service`: Room notification services
- `Flyingdarts.Persistence`: Data persistence layer

**External Dependencies:**
- `Amazon.Lambda.APIGatewayEvents`: API Gateway event types
- `Amazon.Lambda.RuntimeSupport`: Lambda runtime support
- `Amazon.Lambda.Serialization.SystemTextJson`: JSON serialization
- `MediatR`: Mediator pattern implementation
- `Microsoft.Extensions.DependencyInjection`: Dependency injection
- `FluentValidation`: Request validation
- `AWSSDK.DynamoDBv2`: DynamoDB SDK
- `AWSSDK.ApiGatewayManagementApi`: API Gateway Management API

## Development

### Project Structure

```
api/
├── Function.cs                    # Lambda entry point
├── X01ApiBootstrap.cs            # Main bootstrap class
├── ServiceFactory.cs             # Dependency injection setup
├── Models/                       # Data models
│   ├── Connectable.cs            # Base connectable class
│   └── IConnectable.cs           # Connectable interface
├── Requests/                     # Request handlers
│   ├── Create/                   # Game creation requests
│   ├── Join/                     # Game joining requests
│   └── Score/                    # Score update requests
├── Flyingdarts.Backend.Games.X01.Api.csproj  # Project file
└── README.md                     # This documentation
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

## Related Projects

- **flyingdarts-connection-services**: WebSocket connection management
- **flyingdarts-meetings-service**: Meeting and room coordination
- **flyingdarts-notify-rooms-service**: Real-time notifications
- **flyingdarts-persistence**: Data persistence layer
- **flyingdarts-lambda-core**: Core Lambda infrastructure

## Troubleshooting

### Common Issues

1. **WebSocket Connection Failures**: Check API Gateway WebSocket configuration
2. **DynamoDB Access**: Verify IAM permissions for DynamoDB tables
3. **Cold Start Performance**: Consider using provisioned concurrency
4. **Request Validation**: Ensure all required fields are provided in requests

### Debugging

Enable detailed logging:

```bash
export LOG_LEVEL=Debug
```

Check CloudWatch logs for detailed execution information.

### Performance

- **Memory**: Configure appropriate memory allocation (512MB recommended)
- **Timeout**: Set timeout based on expected game session duration
- **Concurrency**: Monitor concurrent WebSocket connections
- **DynamoDB**: Use appropriate read/write capacity units

## Security Considerations

- **Authentication**: All requests require valid user authentication
- **Authorization**: Verify user permissions for game operations
- **Input Validation**: Validate all incoming request data
- **Connection Security**: Use WSS (WebSocket Secure) in production
- **Data Privacy**: Ensure game data is properly isolated per user

## Game Rules

### X01 Game Format

The API supports standard X01 darts game rules:

- **Game Types**: 301, 501, 701 (configurable)
- **Scoring**: Standard darts scoring (1-20, 25, 50)
- **Winning**: Must finish with a double
- **Bust**: Going over the target score results in a bust

### Game Flow

1. **Game Creation**: Player creates a new game session
2. **Player Joining**: Additional players join the game
3. **Game Start**: Game begins when minimum players join
4. **Scoring**: Players record their scores turn by turn
5. **Game End**: Game ends when a player reaches exactly zero with a double

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow MediatR patterns for new commands/queries

## License

Part of the Flyingdarts Turbo monorepo. See root LICENSE file for details.
