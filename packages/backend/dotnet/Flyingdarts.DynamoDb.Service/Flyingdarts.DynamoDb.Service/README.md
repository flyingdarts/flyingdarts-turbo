# Flyingdarts.DynamoDb.Service

## Overview

The Flyingdarts.DynamoDb.Service package provides a comprehensive data access layer for DynamoDB operations in the Flying Darts Turbo platform. This package abstracts DynamoDB interactions and provides a clean, type-safe interface for managing game data, user information, and application state.

The package is designed to:
- Provide a unified interface for DynamoDB operations
- Handle game data management (games, players, darts)
- Manage user data and authentication
- Support efficient querying with proper indexing
- Implement error handling and custom exceptions
- Provide async operations with cancellation token support
- Integrate with the persistence layer for data models

## Features

- **Game Management**: Complete CRUD operations for games, players, and darts
- **User Management**: User data operations with multiple lookup methods
- **Efficient Querying**: Optimized DynamoDB queries with proper indexing
- **Type Safety**: Strongly-typed operations with nullable reference types
- **Error Handling**: Custom exceptions for specific error scenarios
- **Async Operations**: Full async/await support with cancellation tokens
- **Dependency Injection**: Clean interface design for easy testing
- **Performance Optimized**: Efficient DynamoDB operations with proper configuration

## Prerequisites

- .NET 8.0 or later
- AWS DynamoDB service
- AWS SDK for DynamoDB
- Flyingdarts.Persistence package
- Microsoft.Extensions.Options

## Installation

Add the package to your project file:

```xml
<ItemGroup>
  <ProjectReference Include="../../packages/backend/dotnet/Flyingdarts.DynamoDb.Service/Flyingdarts.DynamoDb.Service/Flyingdarts.DynamoDb.Service.csproj" />
</ItemGroup>
```

Or reference it in your solution:

```xml
<ProjectReference Include="packages/backend/dotnet/Flyingdarts.DynamoDb.Service/Flyingdarts.DynamoDb.Service/Flyingdarts.DynamoDb.Service.csproj" />
```

## Usage

### Basic Service Registration

```csharp
using Flyingdarts.DynamoDb.Service;

// Register the service in your DI container
services.AddScoped<IDynamoDbService, DynamoDbService>();

// Configure AWS DynamoDB context
services.AddAWSService<IAmazonDynamoDB>();
services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
```

### Game Operations

```csharp
using Flyingdarts.DynamoDb.Service;

public class GameService
{
    private readonly IDynamoDbService _dynamoDbService;
    
    public GameService(IDynamoDbService dynamoDbService)
    {
        _dynamoDbService = dynamoDbService;
    }
    
    public async Task<Game> CreateGameAsync(Game game)
    {
        await _dynamoDbService.WriteGameAsync(game, CancellationToken.None);
        return game;
    }
    
    public async Task<List<Game>> GetGamesAsync(long gameId)
    {
        return await _dynamoDbService.ReadGameAsync(gameId, CancellationToken.None);
    }
    
    public async Task<Game?> GetOpenGameAsync(long userId)
    {
        return await _dynamoDbService.GetOpenGameByUserIdAsync(userId, CancellationToken.None);
    }
    
    public async Task<List<GamePlayer>> GetGamePlayersAsync(long gameId)
    {
        return await _dynamoDbService.ReadGamePlayersAsync(gameId, CancellationToken.None);
    }
    
    public async Task<List<GameDart>> GetGameDartsAsync(long gameId)
    {
        return await _dynamoDbService.ReadGameDartsAsync(gameId, CancellationToken.None);
    }
}
```

### User Operations

```csharp
using Flyingdarts.DynamoDb.Service;

public class UserService
{
    private readonly IDynamoDbService _dynamoDbService;
    
    public UserService(IDynamoDbService dynamoDbService)
    {
        _dynamoDbService = dynamoDbService;
    }
    
    public async Task<User> GetUserAsync(string userId)
    {
        try
        {
            return await _dynamoDbService.ReadUserAsync(userId, CancellationToken.None);
        }
        catch (UserNotFoundException ex)
        {
            // Handle user not found
            throw new ApplicationException($"User not found: {ex.UserId}");
        }
    }
    
    public async Task<User> GetUserByAuthProviderAsync(string authProviderUserId)
    {
        return await _dynamoDbService.ReadUserByAuthProviderUserIdAsync(
            authProviderUserId, 
            CancellationToken.None
        );
    }
    
    public async Task<User?> GetUserByConnectionIdAsync(string connectionId)
    {
        return await _dynamoDbService.ReadUserByConnectionIdAsync(
            connectionId, 
            CancellationToken.None
        );
    }
    
    public async Task<List<User>> GetMultipleUsersAsync(string[] userIds)
    {
        return await _dynamoDbService.ReadUsersAsync(userIds, CancellationToken.None);
    }
    
    public async Task UpdateUserAsync(User user)
    {
        await _dynamoDbService.WriteUserAsync(user, CancellationToken.None);
    }
}
```

### Error Handling

```csharp
public async Task<User> GetUserWithErrorHandling(string userId)
{
    try
    {
        return await _dynamoDbService.ReadUserAsync(userId, CancellationToken.None);
    }
    catch (UserNotFoundException ex)
    {
        Console.WriteLine($"User not found by {ex.SearchParam}: {ex.UserId}");
        // Handle user not found scenario
        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database operation failed: {ex.Message}");
        // Handle other database errors
        throw;
    }
}
```

### Lambda Function Integration

```csharp
using Flyingdarts.DynamoDb.Service;
using Amazon.Lambda.APIGatewayEvents;

public class GameHandler
{
    private readonly IDynamoDbService _dynamoDbService;
    
    public GameHandler(IDynamoDbService dynamoDbService)
    {
        _dynamoDbService = dynamoDbService;
    }
    
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request)
    {
        try
        {
            var gameId = long.Parse(request.PathParameters["gameId"]);
            var games = await _dynamoDbService.ReadGameAsync(gameId, CancellationToken.None);
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(games)
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = $"Error: {ex.Message}"
            };
        }
    }
}
```

## API Reference

### Interfaces

#### IDynamoDbService

The main interface for DynamoDB operations.

**Game Operations:**

##### ReadGameAsync(long, CancellationToken): Task<List<Game>>
Retrieves all games for a given game ID.

**Parameters:**
- `gameId` (long): The game identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<List<Game>>`: List of games

##### GetOpenGameByUserIdAsync(long, CancellationToken): Task<Game?>
Retrieves the most recent open game for a user.

**Parameters:**
- `userId` (long): The user identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<Game?>`: The open game or null if not found

##### ReadStartedGameAsync(long, CancellationToken): Task<List<Game>>
Retrieves all started games for a given game ID.

**Parameters:**
- `gameId` (long): The game identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<List<Game>>`: List of started games

##### ReadGamePlayersAsync(long, CancellationToken): Task<List<GamePlayer>>
Retrieves all players for a given game.

**Parameters:**
- `gameId` (long): The game identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<List<GamePlayer>>`: List of game players

##### ReadGameDartsAsync(long, CancellationToken): Task<List<GameDart>>
Retrieves all darts for a given game.

**Parameters:**
- `gameId` (long): The game identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<List<GameDart>>`: List of game darts

**User Operations:**

##### ReadUserAsync(string, CancellationToken): Task<User>
Retrieves a user by user ID.

**Parameters:**
- `userId` (string): The user identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<User>`: The user

**Exceptions:**
- `UserNotFoundException`: Thrown when user is not found

##### ReadUserByAuthProviderUserIdAsync(string, CancellationToken): Task<User>
Retrieves a user by authentication provider user ID.

**Parameters:**
- `authProviderUserId` (string): The auth provider user identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<User>`: The user

##### ReadUserByConnectionIdAsync(string, CancellationToken): Task<User?>
Retrieves a user by WebSocket connection ID.

**Parameters:**
- `connectionId` (string): The connection identifier
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<User?>`: The user or null if not found

##### ReadUsersAsync(string[], CancellationToken): Task<List<User>>
Retrieves multiple users by their IDs.

**Parameters:**
- `userIds` (string[]): Array of user identifiers
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<List<User>>`: List of users

##### ReadAllUsersAsync(CancellationToken): Task<List<User>>
Retrieves all users in the system.

**Parameters:**
- `cancellationToken` (CancellationToken): Token for cancelling the operation

**Returns:**
- `Task<List<User>>`: List of all users

**Write Operations:**

##### WriteUserAsync(User, CancellationToken): Task
Writes a user to DynamoDB.

**Parameters:**
- `user` (User): The user to write
- `cancellationToken` (CancellationToken): Token for cancelling the operation

##### WriteGameAsync(Game, CancellationToken): Task
Writes a game to DynamoDB.

**Parameters:**
- `game` (Game): The game to write
- `cancellationToken` (CancellationToken): Token for cancelling the operation

##### WriteGamePlayerAsync(GamePlayer, CancellationToken): Task
Writes a game player to DynamoDB.

**Parameters:**
- `player` (GamePlayer): The game player to write
- `cancellationToken` (CancellationToken): Token for cancelling the operation

##### WriteGameDartAsync(GameDart, CancellationToken): Task
Writes a game dart to DynamoDB.

**Parameters:**
- `dart` (GameDart): The game dart to write
- `cancellationToken` (CancellationToken): Token for cancelling the operation

##### PutGameAsync(Game, CancellationToken): Task
Performs a put operation for a game (overwrites existing data).

**Parameters:**
- `game` (Game): The game to put
- `cancellationToken` (CancellationToken): Token for cancelling the operation

### Classes

#### DynamoDbService

The concrete implementation of the DynamoDB service.

**Properties:**
- `DbContext` (IDynamoDBContext): The DynamoDB context
- `OperationConfig` (DynamoDBOperationConfig): The operation configuration

**Constructor:**
- `DynamoDbService(IDynamoDBContext, IOptions<ApplicationOptions>)`: Creates a new service instance

#### UserNotFoundException

Custom exception thrown when a user is not found.

**Properties:**
- `SearchParam` (string): The search parameter used
- `UserId` (string): The user ID that was searched for

**Constructor:**
- `UserNotFoundException(string, string)`: Creates a new exception instance

## Configuration

The service uses the following configuration options:

- **Target Framework**: .NET 8.0
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled
- **DynamoDB Context**: Configured via dependency injection
- **Application Options**: Configuration for DynamoDB operations

### Dependency Injection Setup

```csharp
// In your Startup.cs or Program.cs
services.AddScoped<IDynamoDbService, DynamoDbService>();

// Configure AWS services
services.AddAWSService<IAmazonDynamoDB>();
services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

// Configure application options
services.Configure<ApplicationOptions>(configuration.GetSection("Application"));
```

## Database Schema

The service works with the following DynamoDB table structure:

### Users Table
- **Primary Key**: `UserId` (string)
- **GSI**: `AuthProviderUserId` (string)
- **GSI**: `ConnectionId` (string)
- **Attributes**:
  - `AuthProviderUserId` (string): Authentication provider user ID
  - `ConnectionId` (string): WebSocket connection ID
  - `Name` (string): User display name
  - `Email` (string): User email address
  - Additional user attributes

### Games Table
- **Primary Key**: `GameId` (string)
- **Sort Key**: `CreationDate` (string)
- **GSI**: `GameCreator` (string)
- **Attributes**:
  - `Status` (string): Game status (Qualifying, Started, Finished)
  - `GameCreator` (string): User ID of game creator
  - `Players` (list): List of player IDs
  - Additional game attributes

### GamePlayers Table
- **Primary Key**: `GameId` (string)
- **Sort Key**: `PlayerId` (string)
- **Attributes**:
  - `PlayerId` (string): User ID of the player
  - `Score` (number): Current player score
  - `Position` (number): Player position in game
  - Additional player attributes

### GameDarts Table
- **Primary Key**: `GameId` (string)
- **Sort Key**: `Timestamp` (string)
- **Attributes**:
  - `PlayerId` (string): User ID of the player
  - `Score` (number): Dart score
  - `Multiplier` (number): Score multiplier (1, 2, 3)
  - `Position` (number): Dart position on board
  - Additional dart attributes

## Development

### Building the Package

```bash
dotnet build Flyingdarts.DynamoDb.Service.csproj
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
- **Flyingdarts.Persistence**: Data persistence models and utilities

### External Dependencies
- **AWSSDK.DynamoDBv2** (4.0.3): AWS DynamoDB SDK

## Related Projects

- **Flyingdarts.Persistence**: Data persistence models
- **Flyingdarts.Core**: Core models and utilities
- **Flyingdarts.Connection.Services**: WebSocket connection management
- **Flyingdarts.Lambda.Core**: Lambda function infrastructure

## Contributing

When contributing to this package:

1. Follow the existing code style and patterns
2. Add XML documentation for all public APIs
3. Include unit tests for new functionality
4. Ensure proper error handling and validation
5. Test with real DynamoDB integration
6. Update this README for any new features or breaking changes
7. Consider performance implications of new queries

## License

This package is part of the Flying Darts Turbo monorepo and follows the same licensing terms.