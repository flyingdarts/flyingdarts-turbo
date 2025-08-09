# Flying Darts Persistence

## Overview

The Flying Darts Persistence is a .NET 8 NuGet package that provides comprehensive data persistence models and services for the Flying Darts gaming platform. This package contains all the core domain models, DynamoDB integration patterns, and data access abstractions needed for the darts gaming application.

The package implements a robust data layer with AWS DynamoDB integration, featuring optimized table designs, caching services, queue management, and comprehensive domain models for games, players, darts, users, and social features. It provides a clean abstraction layer that enables efficient data operations while maintaining type safety and performance.

## Features

- **Domain Models**: Complete set of domain entities for darts gaming
- **DynamoDB Integration**: Native AWS DynamoDB support with optimized table designs
- **Caching Service**: High-performance caching layer for game state management
- **Queue Management**: Generic queue service for managing game queues
- **Social Features**: Friend relationships and friend request models
- **Game State Management**: Comprehensive game state tracking and persistence
- **User Management**: User profiles and authentication integration
- **Type Safety**: Strongly typed models with proper validation
- **Performance Optimization**: Optimized queries and batch operations
- **Extensible Design**: Generic services for different entity types

## Prerequisites

- **.NET 8 SDK**: Required for building and using the library
- **AWS DynamoDB**: Access to DynamoDB tables and services
- **AWS SDK**: AWS SDK for .NET for DynamoDB operations
- **DynamoDB Tables**: Properly configured DynamoDB tables with correct schemas

## Installation

### NuGet Package
```bash
dotnet add package Flyingdarts.Persistence
```

### From Source
1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the persistence package**:
   ```bash
   cd packages/backend/dotnet/Flyingdarts.Persistence
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

1. **Add the package to your project**:
   ```xml
   <PackageReference Include="Flyingdarts.Persistence" Version="1.3.6" />
   ```

2. **Configure DynamoDB services**:
   ```csharp
   services.AddScoped<IDynamoDBContext, DynamoDBContext>();
   services.AddScoped<IAmazonDynamoDB, AmazonDynamoDBClient>();
   services.Configure<ApplicationOptions>(configuration.GetSection("Application"));
   ```

### Working with Domain Models

#### Creating a Game
```csharp
public class GameService
{
    public Game CreateX01Game(int playerCount, X01GameSettings settings, Guid meetingIdentifier)
    {
        return Game.Create(playerCount, settings, meetingIdentifier);
    }
}
```

#### Creating a User
```csharp
public class UserService
{
    public User CreateUser(string authProviderUserId, string connectionId, UserProfile profile)
    {
        return User.Create(authProviderUserId, connectionId, profile);
    }
}
```

#### Recording a Dart Throw
```csharp
public class DartService
{
    public GameDart RecordDart(long gameId, string playerId, int score, int gameScore, int set, int leg)
    {
        return GameDart.Create(gameId, playerId, score, gameScore, set, leg);
    }
}
```

### Using the Caching Service

```csharp
public class GameStateService
{
    private readonly CachingService<X01State> _cachingService;

    public GameStateService(CachingService<X01State> cachingService)
    {
        _cachingService = cachingService;
    }

    public async Task LoadGameState(string gameId)
    {
        await _cachingService.Load(gameId, CancellationToken.None);
    }

    public async Task SaveGameState()
    {
        await _cachingService.Save(CancellationToken.None);
    }

    public void AddDartToGame(GameDart dart)
    {
        _cachingService.AddDart(dart);
    }
}
```

### Using the Queue Service

```csharp
public class QueueService<T> where T : ISortKeyItem
{
    private readonly QueueService<T> _queueService;

    public async Task AddToQueue(T item)
    {
        await _queueService.AddRecord(item, CancellationToken.None);
    }

    public async Task<List<T>> GetQueueItems()
    {
        return await _queueService.GetRecords(CancellationToken.None);
    }

    public async Task RemoveFromQueue(IEnumerable<T> items)
    {
        await _queueService.DeleteRecords(items, CancellationToken.None);
    }
}
```

## API Reference

### Core Domain Models

#### Game
Represents a darts game with all its properties and settings.

```csharp
public class Game : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    public string PrimaryKey { get; set; }        // "GAME#HEADER"
    public string SortKey { get; set; }           // "{GameId}#{Status}"
    public string LSI1 { get; set; }              // "{Status}#{GameId}"
    public long GameId { get; set; }
    public GameType Type { get; set; }
    public GameStatus Status { get; set; }
    public int PlayerCount { get; set; }
    public X01GameSettings X01 { get; set; }
    public DateTime CreationDate { get; }
    public Guid MeetingIdentifier { get; set; }

    public static Game Create(int playerCount, X01GameSettings settings, Guid meetingIdentifier);
}
```

#### User
Represents a user in the system with profile and connection information.

```csharp
public class User : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    public string PrimaryKey { get; set; }        // "FD#USER"
    public string SortKey { get; set; }           // "{UserId}#{Country}"
    public string LSI1 { get; set; }              // "{AuthProviderUserId}#{CreatedAt}"
    public string UserId { get; set; }
    public string ConnectionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AuthProviderUserId { get; set; }
    public UserProfile Profile { get; set; }
    public Guid MeetingIdentifier { get; set; }

    public static User Create(string authProviderUserId, string connectionId, UserProfile userProfile);
}
```

#### GameDart
Represents a single dart throw in a game.

```csharp
public class GameDart : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    public string PrimaryKey { get; set; }        // "GAME#DART"
    public string SortKey { get; set; }           // "{GameId}#{Id}#{PlayerId}"
    public string LSI1 { get; set; }              // "{PlayerId}#{CreatedAt}"
    public Guid Id { get; set; }
    public long GameId { get; set; }
    public string PlayerId { get; set; }
    public int Score { get; set; }
    public int GameScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Leg { get; set; }
    public int Set { get; set; }

    public static GameDart Create(long gameId, string playerId, int score, int gameScore, int set, int leg);
}
```

#### FriendRelationship
Represents a friendship between two users.

```csharp
public class FriendRelationship : IPrimaryKeyItem, ISortKeyItem, IAlternativeSortKeyItem
{
    public string PrimaryKey { get; set; }        // "FRIEND#RELATIONSHIP"
    public string SortKey { get; set; }           // "{RequesterId}#{FriendId}"
    public string LSI1 { get; set; }              // "{FriendId}#{RequesterId}"
    public string RequesterId { get; set; }
    public string FriendId { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string? RequestMessage { get; set; }

    public static FriendRelationship Create(string requesterId, string targetUserId, DateTime requestTime, string message);
}
```

### Services

#### CachingService<T>
Generic caching service for game state management.

```csharp
public class CachingService<T> : ICachingService<T>
    where T : IGameState<T>
{
    public T State { get; set; }
    
    public async Task Load(string gameId, CancellationToken cancellationToken);
    public async Task Save(CancellationToken cancellationToken);
    public void CreateInitial(T state, Game game);
    public void AddGame(Game game);
    public void AddPlayer(GamePlayer player);
    public void AddDart(GameDart dart);
    public void AddUser(User user);
}
```

#### QueueService<T>
Generic queue service for managing collections of items.

```csharp
public class QueueService<T> : IQueueService<T>
    where T : ISortKeyItem
{
    public async Task DeleteRecords(IEnumerable<T> records, CancellationToken cancellationToken);
    public async Task AddRecord(T record, CancellationToken cancellationToken);
    public async Task<List<T>> GetRecords(CancellationToken cancellationToken);
}
```

### Interfaces

#### IPrimaryKeyItem
Interface for items with a primary key.

```csharp
public interface IPrimaryKeyItem
{
    string PrimaryKey { get; set; }
}
```

#### ISortKeyItem
Interface for items with a sort key.

```csharp
public interface ISortKeyItem
{
    string SortKey { get; set; }
}
```

#### IAlternativeSortKeyItem
Interface for items with an alternative sort key (LSI).

```csharp
public interface IAlternativeSortKeyItem
{
    string LSI1 { get; set; }
}
```

#### IGameState<T>
Interface for game state objects.

```csharp
public interface IGameState<T>
{
    Game? Game { get; set; }
    List<GamePlayer>? Players { get; set; }
    List<GameDart>? Darts { get; set; }
    List<User>? Users { get; set; }
}
```

## Configuration

### DynamoDB Table Configuration
The package expects DynamoDB tables with the following structure:

```json
{
  "TableName": "Flyingdarts-Application-Table",
  "PrimaryKey": "PK",
  "SortKey": "SK",
  "LocalSecondaryIndex": "LSI1"
}
```

### Environment Variables
The package uses the following environment variables:

- **TableName**: DynamoDB table name
- **EnvironmentName**: Environment name for table naming
- **DyteOrganizationId**: Dyte organization ID
- **DyteApiKey**: Dyte API key

### Application Options
Configure the application options in your DI container:

```csharp
services.Configure<ApplicationOptions>(configuration.GetSection("Application"));
```

## Development

### Project Structure
```
Flyingdarts.Persistence/
├── Domain Models/                    # Core domain entities
│   ├── Game.cs                      # Game entity
│   ├── User.cs                      # User entity
│   ├── GameDart.cs                  # Dart throw entity
│   ├── GamePlayer.cs                # Game player entity
│   ├── UserProfile.cs               # User profile entity
│   ├── FriendRelationship.cs        # Friend relationship entity
│   ├── FriendRequest.cs             # Friend request entity
│   └── X01GameSettings.cs           # X01 game settings
├── Services/                        # Service implementations
│   ├── CachingService.cs            # Game state caching
│   └── QueueService.cs              # Queue management
├── Interfaces/                      # Service interfaces
│   ├── ICachingService.cs           # Caching service interface
│   ├── IQueueService.cs             # Queue service interface
│   ├── IGameState.cs                # Game state interface
│   └── IGameSettings.cs             # Game settings interface
├── Enums/                          # Enumeration types
│   ├── GameType.cs                  # Game type enumeration
│   ├── GameStatus.cs                # Game status enumeration
│   ├── FriendRequestStatus.cs       # Friend request status
│   └── FriendshipStatus.cs          # Friendship status
├── Configuration/                   # Configuration classes
│   └── ApplicationOptions.cs        # Application configuration
├── Constants.cs                     # DynamoDB constants
├── GlobalUsings.cs                  # Global using directives
└── Flyingdarts.Persistence.csproj   # Project file
```

### Architecture Patterns
- **Domain-Driven Design**: Rich domain models with business logic
- **Repository Pattern**: Data access abstraction through services
- **Generic Pattern**: Reusable services for different entity types
- **Caching Pattern**: Performance optimization through caching
- **Queue Pattern**: Asynchronous processing and state management

### DynamoDB Design Patterns

#### Single Table Design
The package uses a single-table DynamoDB design with the following key patterns:

- **Primary Key (PK)**: Entity type identifier (e.g., "GAME#HEADER", "FD#USER")
- **Sort Key (SK)**: Entity-specific identifier with additional context
- **Local Secondary Index (LSI1)**: Alternative access patterns for queries

#### Key Examples
- **Games**: `PK="GAME#HEADER"`, `SK="{GameId}#{Status}"`
- **Users**: `PK="FD#USER"`, `SK="{UserId}#{Country}"`
- **Darts**: `PK="GAME#DART"`, `SK="{GameId}#{Id}#{PlayerId}"`
- **Friends**: `PK="FRIEND#RELATIONSHIP"`, `SK="{RequesterId}#{FriendId}"`

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

### External Dependencies
- **AWSSDK.Core** (4.0.0.16): AWS SDK core functionality
- **AWSSDK.DynamoDBv2** (4.0.3): DynamoDB client and models
- **AWSSDK.SimpleSystemsManager** (4.0.2.4): AWS Systems Manager integration
- **Microsoft.Extensions.Configuration** (10.0.0-preview.6.25358.103): Configuration management
- **Microsoft.Extensions.Options** (10.0.0-preview.6.25358.103): Options pattern support

### Internal Dependencies
This is a standalone package with no internal dependencies.

## Related Projects

### Backend Services
- **[Friends API](../../../apps/backend/dotnet/friends/)**: Friend management and social features
- **[Games API](../../../apps/backend/dotnet/games/)**: Game management and scoring
- **[Auth API](../../../apps/backend/dotnet/auth/)**: Authentication and authorization

### Frontend Applications
- **[Flutter Mobile App](../../../../frontend/flutter/flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../frontend/angular/fd-app/)**: Web application

### Shared Packages
- **[Core Package](../../../Flyingdarts.Core/)**: Shared business logic
- **[DynamoDB Service](../../../Flyingdarts.DynamoDb.Service/)**: Database operations
- **[Metadata Services](../../../Flyingdarts.Metadata.Services/)**: Metadata generation

## Version History

- **v1.3.6**: PlayerId now behaves like GameId, equals CreatedAt = DateTime.UtcNow.Ticks
- **v0.0.9** (2025-07-27): Various updates
- **v0.0.8** (2025-07-26): Implemented friends feature
- **v0.0.7** (2025-07-19): Pipeline up and running
- **v0.0.6** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.5** (2025-07-10): Re-organize abit
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Maintain DynamoDB design patterns
6. Update package version and release notes

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
