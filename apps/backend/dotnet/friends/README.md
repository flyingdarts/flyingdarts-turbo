# Flying Darts Friends API

## Overview

The Flying Darts Friends API is a .NET 8 AWS Lambda function that provides comprehensive friend management functionality for the Flying Darts gaming platform. This service handles friend requests, user relationships, and social features that enable players to connect, invite friends to games, and manage their social network within the platform.

The API is built using a clean architecture approach with CQRS (Command Query Responsibility Segregation) pattern, leveraging AWS DynamoDB for data persistence and AWS Lambda for serverless execution.

## Features

- **Friend Management**: Send, accept, and decline friend requests
- **User Search**: Search for users to add as friends
- **Friend List**: Retrieve current user's friends with online status
- **Game Invitations**: Invite friends to join games
- **Request Management**: View incoming and outgoing friend requests
- **Real-time Status**: Track friend online status and active games
- **User Profiles**: Access basic user information (username, country, profile picture)

## Prerequisites

- **.NET 8 SDK**: Required for building and running the application
- **AWS CLI**: For deployment and AWS service interaction
- **AWS Lambda Tools**: For .NET Lambda function deployment
- **DynamoDB**: Database service for data persistence
- **AWS IAM**: Proper permissions for Lambda execution and DynamoDB access

## Installation

1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd flyingdarts-turbo
   ```

2. **Navigate to the friends project**:
   ```bash
   cd apps/backend/dotnet/friends
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

### Local Development

1. **Build for development**:
   ```bash
   npm run build:dev
   ```

2. **Run tests** (if available):
   ```bash
   dotnet test
   ```

### Deployment

1. **Build for production**:
   ```bash
   npm run build:prod
   ```

2. **Deploy to development environment**:
   ```bash
   npm run deploy:dev
   ```

3. **Deploy to production environment**:
   ```bash
   npm run deploy:prod
   ```

## API Reference

### Authentication
All endpoints require authentication via AWS API Gateway authorizer. The user ID is extracted from the JWT token and passed to the Lambda function.

### Endpoints

#### User Management
- `GET /friends/user` - Get current user information

#### Friend Management
- `GET /friends` - Get user's friends list
- `DELETE /friends/{friendId}` - Remove a friend

#### Friend Requests
- `GET /friends/requests` - Get incoming friend requests
- `POST /friends/request` - Send a friend request
- `PUT /friends/request/{requestId}` - Respond to a friend request (accept/decline)

#### User Search
- `POST /friends/search` - Search for users to add as friends

#### Game Invitations
- `POST /friends/invite` - Invite a friend to join a game

### Data Models

#### FriendDto
Represents a friend of the current user.
```csharp
public class FriendDto
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Country { get; set; }
    public DateTime FriendsSince { get; set; }
    public bool IsOnline { get; set; }
    public string? ConnectionId { get; set; }
    public string? Picture { get; set; }
    public string? OpenGameId { get; set; }
}
```

#### FriendRequestDto
Represents a friend request sent by the current user.
```csharp
public class FriendRequestDto
{
    public string RequestId { get; set; }
    public string RequesterId { get; set; }
    public string RequesterUserName { get; set; }
    public string TargetUserId { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public FriendRequestStatus Status { get; set; }
}
```

#### UserSearchDto
Represents a user search result.
```csharp
public class UserSearchDto
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Country { get; set; }
    public bool IsAlreadyFriend { get; set; }
    public bool HasPendingRequest { get; set; }
    public string Picture { get; set; }
}
```

## Configuration

### AWS Lambda Configuration
The function is configured with the following settings:
- **Runtime**: .NET 8
- **Memory**: 256 MB
- **Timeout**: 30 seconds
- **Region**: eu-west-1

### Environment Variables
- DynamoDB table names and configuration
- AWS region settings
- Application-specific options

### DynamoDB Schema
The service uses DynamoDB with the following key patterns:
- **Friend Relationships**: `FRIEND#RELATIONSHIP` with composite keys
- **Friend Requests**: `FRIEND#REQUEST` with request IDs
- **Users**: `USER#` with user IDs

## Development

### Project Structure
```
friends/
├── Function.cs                    # Lambda function entry point
├── InnerHandler.cs               # Request routing and handling
├── ServiceFactory.cs             # Dependency injection setup
├── Models/                       # Data transfer objects
│   └── FriendDto.cs
├── Requests/                     # CQRS commands and queries
│   ├── Commands/                 # Write operations
│   │   ├── SendFriendRequest/
│   │   ├── RespondToFriendRequest/
│   │   ├── RemoveFriend/
│   │   └── InviteFriendToGame/
│   └── Queries/                  # Read operations
│       ├── GetUser/
│       ├── GetFriends/
│       ├── GetFriendRequests/
│       └── SearchUsers/
├── Services/                     # Business logic services
│   └── FriendsDynamoDbService.cs
└── aws-lambda-tools-defaults.json
```

### Architecture Patterns
- **CQRS**: Separates read and write operations
- **MediatR**: Implements command/query pattern
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Service lifecycle management

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

## Dependencies

### Internal Dependencies
- **Flyingdarts.Core**: Core business logic and models
- **Flyingdarts.Persistence**: Data persistence abstractions
- **Flyingdarts.DynamoDb.Service**: DynamoDB service implementations

### External Dependencies
- **Amazon.Lambda.APIGatewayEvents** (2.7.1): API Gateway integration
- **Amazon.Lambda.Core** (2.7.0): Lambda runtime support
- **Amazon.Lambda.RuntimeSupport** (1.13.1): .NET Lambda runtime
- **Amazon.Lambda.Serialization.SystemTextJson** (2.4.4): JSON serialization
- **AWSSDK.DynamoDBv2** (4.0.3): DynamoDB client
- **FluentValidation** (12.0.0): Input validation
- **MediatR** (12.5.0): CQRS implementation
- **Microsoft.Extensions.DependencyInjection** (10.0.0-preview.6.25358.103): DI container

## Related Projects

### Backend Services
- **[Auth API](../auth/)**: Authentication and authorization
- **[Games API](../games/)**: Game management and scoring
- **[Signalling API](../signalling/)**: Real-time communication

### Frontend Applications
- **[Flutter Mobile App](../../../frontend/flutter/flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../frontend/angular/fd-app/)**: Web application

### Shared Packages
- **[Core Package](../../../../packages/backend/dotnet/Flyingdarts.Core/)**: Shared business logic
- **[Persistence Package](../../../../packages/backend/dotnet/Flyingdarts.Persistence/)**: Data access layer
- **[DynamoDB Service](../../../../packages/backend/dotnet/Flyingdarts.DynamoDb.Service/)**: Database operations

## Version History

- **v0.0.3** (2025-07-27): Various updates and improvements
- **v0.0.2** (2025-07-26): Implemented friends feature
- **v0.0.1**: Initial implementation

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
