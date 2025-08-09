# Flying Darts Metadata Services

## Overview

The Flying Darts Metadata Services is a .NET 8 library that provides comprehensive metadata generation and aggregation for darts games in the Flying Darts platform. This service is responsible for creating unified, real-time views of game state by combining data from multiple sources including games, players, darts, and users.

The service implements a generic metadata architecture that can be extended to support different game types, with a specific implementation for X01 games. It leverages caching services for performance optimization and provides rich metadata including game progression, player statistics, and real-time game state information.

## Features

- **Game Metadata Generation**: Comprehensive metadata aggregation for darts games
- **X01 Game Support**: Specialized metadata service for X01 game format
- **Real-time State Tracking**: Live game state updates with player progression
- **Performance Optimization**: Caching layer for efficient data retrieval
- **Player Statistics**: Detailed player performance metrics and statistics
- **Game Progression Tracking**: Leg, set, and match progression monitoring
- **Winner Detection**: Automatic detection of game winners and completion
- **Meeting Integration**: Video meeting token and identifier management
- **Extensible Architecture**: Generic base class for supporting multiple game types

## Prerequisites

- **.NET 8 SDK**: Required for building and using the library
- **DynamoDB Access**: Access to DynamoDB tables for game data
- **Caching Service**: Caching infrastructure for performance optimization
- **AWS Systems Manager**: For configuration management (optional)

## Installation

1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the metadata services**:
   ```bash
   cd packages/backend/dotnet/Flyingdarts.Metadata.Services/Flyingdarts.Metadata.Services
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
   services.AddScoped<X01MetadataService>();
   services.AddScoped<CachingService<X01State>>();
   ```

2. **Configure caching service**:
   ```csharp
   services.AddScoped<CachingService<X01State>>(provider =>
   {
       var dynamoDbService = provider.GetRequiredService<IDynamoDbService>();
       return new CachingService<X01State>(dynamoDbService);
   });
   ```

### Retrieving Game Metadata

```csharp
public class GameController
{
    private readonly X01MetadataService _metadataService;

    public GameController(X01MetadataService metadataService)
    {
        _metadataService = metadataService;
    }

    public async Task<IActionResult> GetGameMetadata(string gameId, string? userId)
    {
        var metadata = await _metadataService.GetMetadataAsync(
            gameId, 
            userId, 
            CancellationToken.None
        );
        
        return Ok(metadata);
    }
}
```

### Working with Metadata

```csharp
// Access game information
var gameInfo = metadata.Game;
var gameType = gameInfo.Type;
var gameStatus = gameInfo.Status;

// Access player information
var players = metadata.Players;
var nextPlayer = metadata.NextPlayer;
var winningPlayer = metadata.WinningPlayer;

// Access darts information
var playerDarts = metadata.Darts;
var meetingToken = metadata.MeetingToken;

// Convert to dictionary for serialization
var metadataDict = metadata.ToDictionary();
```

## API Reference

### Core Services

#### MetadataService<T>
Generic base class for metadata services.

```csharp
public abstract class MetadataService<T>
    where T : IGameState<T>
{
    private readonly CachingService<T> _cachingService;

    protected MetadataService(CachingService<T> cachingService)
    {
        _cachingService = cachingService;
    }
}
```

#### X01MetadataService
Specialized service for X01 game metadata.

```csharp
public sealed class X01MetadataService : MetadataService<X01State>
{
    public async Task<Dtos.Metadata> GetMetadataAsync(
        string gameId,
        string? userId,
        CancellationToken cancellationToken = default
    );
}
```

### Data Models

#### Metadata
Main metadata container for game information.

```csharp
public class Metadata
{
    public GameDto? Game { get; set; }
    public IOrderedEnumerable<PlayerDto>? Players { get; set; }
    public Dictionary<string, List<DartDto>>? Darts { get; set; }
    public Guid? MeetingIdentifier { get; set; }
    public string? MeetingToken { get; set; }
    public string? NextPlayer { get; set; }
    public string? WinningPlayer { get; set; }
    
    public Dictionary<string, object> ToDictionary();
}
```

#### GameDto
Represents game information in the metadata.

```csharp
public class GameDto
{
    public string? Id { get; set; }
    public GameTypeDto Type { get; set; }
    public GameStatusDto Status { get; set; }
    public int PlayerCount { get; set; }
    public X01GameSettingsDto? X01 { get; set; }
}
```

#### PlayerDto
Represents player information in the metadata.

```csharp
public class PlayerDto
{
    public string? PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? CreatedAt { get; set; }
    public string? Country { get; set; }
    public string? Sets { get; set; }
    public string? Legs { get; set; }
}
```

#### DartDto
Represents a single dart throw in the game metadata.

```csharp
public class DartDto
{
    public Guid Id { get; set; }
    public long GameId { get; set; }
    public string? PlayerId { get; set; }
    public int Score { get; set; }
    public int GameScore { get; set; }
    public long CreatedAt { get; set; }
    public int Leg { get; set; }
    public int Set { get; set; }
}
```

### Service Methods

#### GetMetadataAsync(string gameId, string? userId, CancellationToken cancellationToken)
Retrieves and constructs complete metadata for an X01 game.

- **Parameters**:
  - `gameId` (string): The unique identifier of the game
  - `userId` (string?): Optional user ID for context
  - `cancellationToken` (CancellationToken): Cancellation token
- **Returns**: Task<Dtos.Metadata> - Complete game metadata
- **Throws**: 
  - ArgumentException if game ID format is invalid
  - InvalidOperationException if game is not found

## Configuration

### Environment Variables
The service uses the following environment variables through AWS Systems Manager:

- **DynamoDB Configuration**: Table names and access settings
- **Caching Configuration**: Cache settings and TTL values

### Configuration File
You can configure the service using appsettings.json:

```json
{
  "DynamoDB": {
    "GameTableName": "flyingdarts-games",
    "PlayerTableName": "flyingdarts-players",
    "DartTableName": "flyingdarts-darts"
  },
  "Caching": {
    "DefaultTTL": 300,
    "GameStateTTL": 60
  }
}
```

## Development

### Project Structure
```
Flyingdarts.Metadata.Services/
├── Dtos/                          # Data transfer objects
│   ├── Metadata.cs               # Main metadata container
│   ├── GameDto.cs                # Game information
│   ├── PlayerDto.cs              # Player information
│   ├── DartDto.cs                # Dart throw information
│   ├── X01GameSettingsDto.cs     # X01-specific settings
│   ├── GameStatusDto.cs          # Game status enumeration
│   └── GameTypeDto.cs            # Game type enumeration
├── Services/                      # Service implementations
│   ├── MetadataService.cs        # Generic base service
│   └── X01/                      # X01-specific implementation
│       └── X01MetadataService.cs # X01 metadata service
├── GlobalUsings.cs               # Global using directives
└── Flyingdarts.Metadata.Services.csproj
```

### Architecture Patterns
- **Generic Service Pattern**: Base class for different game types
- **Caching Pattern**: Performance optimization through caching
- **Aggregation Pattern**: Combining data from multiple sources
- **Factory Pattern**: Service creation and configuration
- **Repository Pattern**: Data access through DynamoDB service

### Key Features

#### Game State Aggregation
The service aggregates data from multiple sources:
- **Game Data**: Basic game information and settings
- **Player Data**: Player details and statistics
- **Dart Data**: Individual dart throws and scores
- **User Data**: User profile information

#### Performance Optimization
- **Caching Layer**: Reduces database calls for frequently accessed data
- **Parallel Data Fetching**: Concurrent retrieval of game components
- **Efficient Queries**: Optimized DynamoDB queries for game data

#### Real-time Updates
- **Live Game State**: Real-time tracking of game progression
- **Player Turn Management**: Automatic next player determination
- **Winner Detection**: Instant detection of game completion

### Testing
Run unit tests to ensure code quality:
```bash
dotnet test
```

### Code Quality
- Follow C# coding conventions
- Use XML documentation for public APIs
- Implement proper error handling
- Add logging for debugging
- Use nullable reference types

## Dependencies

### Internal Dependencies
- **Flyingdarts.Persistence**: Data persistence abstractions
- **Flyingdarts.DynamoDb.Service**: DynamoDB service implementations

### External Dependencies
- **AWSSDK.SimpleSystemsManagement** (4.0.2.4): AWS Systems Manager integration
- **Microsoft.Extensions.Configuration** (10.0.0-preview.6.25358.103): Configuration management

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
- **[Persistence Package](../../../Flyingdarts.Persistence/)**: Data access layer
- **[DynamoDB Service](../../../Flyingdarts.DynamoDb.Service/)**: Database operations

## Version History

- **v0.0.8** (2025-07-26): Implemented friends feature
- **v0.0.7** (2025-07-19): Pipeline up and running
- **v0.0.6** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.5** (2025-07-10): Re-organize abit
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution
- **v0.0.2** (2025-07-07): Initial implementation

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Extend the generic MetadataService<T> for new game types
6. Maintain performance optimization patterns

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
