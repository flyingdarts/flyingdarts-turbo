# Flyingdarts.Backend.Friends.Api

## Overview

The Flyingdarts.Backend.Friends.Api service is a .NET 8 AWS Lambda function that provides friends management functionality for the Flyingdarts Turbo platform. This service handles friend requests, friend lists, and social features through REST API endpoints, enabling users to connect and play darts with their friends.

This service is responsible for:
- Managing friend relationships and friend requests
- Handling friend list operations (add, remove, list)
- Processing friend request workflows (send, accept, decline)
- Providing social features for the darts gaming platform
- Integrating with DynamoDB for persistent friend data storage
- Supporting real-time friend status updates

## Features

- **Friend Management**: Complete friend relationship lifecycle management
- **Friend Requests**: Send, accept, and decline friend requests
- **Friend Lists**: Retrieve and manage user friend lists
- **Social Features**: Enable social interactions between players
- **AWS Lambda Integration**: Serverless deployment with automatic scaling
- **MediatR Pattern**: Clean separation of concerns with command/query handlers
- **Dependency Injection**: Flexible service composition and testing
- **Validation**: Request validation using FluentValidation
- **Error Handling**: Comprehensive error handling with proper HTTP responses
- **DynamoDB Integration**: Persistent storage for friend relationships

## Prerequisites

- .NET 8 SDK
- AWS CLI configured with appropriate permissions
- DynamoDB tables configured for friend data
- Visual Studio 2022 or VS Code with C# extensions
- Docker (for containerized deployment)

## Installation

1. Clone the monorepo and navigate to the friends API:
```bash
cd apps/backend/dotnet/friends
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

Run the friends API locally for development:

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
aws lambda update-function-code --function-name flyingdarts-friends-api --zip-file fileb://Flyingdarts.Backend.Friends.Api.zip
```

### REST API Endpoints

The API exposes the following REST endpoints:

#### Friend Requests

##### Send Friend Request
- **Method**: `POST`
- **Route**: `/friends/request`
- **Description**: Send a friend request to another user

##### Accept Friend Request
- **Method**: `POST`
- **Route**: `/friends/accept`
- **Description**: Accept a pending friend request

##### Decline Friend Request
- **Method**: `POST`
- **Route**: `/friends/decline`
- **Description**: Decline a pending friend request

##### Get Friend Requests
- **Method**: `GET`
- **Route**: `/friends/requests`
- **Description**: Get pending friend requests for a user

#### Friend Management

##### Get Friends List
- **Method**: `GET`
- **Route**: `/friends/list`
- **Description**: Get the complete friends list for a user

##### Remove Friend
- **Method**: `DELETE`
- **Route**: `/friends/remove`
- **Description**: Remove a friend from the user's friends list

## API Reference

### Main Classes

#### `Function`

The main Lambda function entry point that handles API Gateway HTTP requests.

**Properties:**
- `ILogger<Function> _logger`: Logging instance for the function
- `IInnerHandler _innerHandler`: Inner handler for processing requests
- `IConfiguration _configuration`: Configuration management

**Methods:**

##### `FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context): Task<APIGatewayProxyResponse>`

Main handler for API Gateway HTTP requests.

**Parameters:**
- `request` (APIGatewayProxyRequest): The HTTP request from API Gateway
- `context` (ILambdaContext): Lambda execution context

**Returns:**
- `Task<APIGatewayProxyResponse>`: HTTP response with appropriate status code and body

**Throws:**
- `ArgumentException`: When request is malformed
- `UnauthorizedAccessException`: When user is not authenticated

#### `InnerHandler`

Inner handler class that processes the actual business logic.

**Methods:**

##### `HandleAsync(APIGatewayProxyRequest request): Task<APIGatewayProxyResponse>`

Processes the incoming request and returns the appropriate response.

**Parameters:**
- `request` (APIGatewayProxyRequest): The HTTP request to process

**Returns:**
- `Task<APIGatewayProxyResponse>`: Processed response

#### `ServiceFactory`

Factory class for setting up dependency injection and service configuration.

**Methods:**

##### `CreateServiceProvider(): IServiceProvider`

Creates and configures the dependency injection container.

**Returns:**
- `IServiceProvider`: Configured service provider

### Request Models

#### Friend Request Models

##### `SendFriendRequestCommand`

Command for sending a friend request.

**Properties:**
- `string UserId` (get; set;): ID of the user sending the request
- `string FriendId` (get; set;): ID of the user to send the request to

##### `AcceptFriendRequestCommand`

Command for accepting a friend request.

**Properties:**
- `string UserId` (get; set;): ID of the user accepting the request
- `string FriendId` (get; set;): ID of the user whose request is being accepted

##### `DeclineFriendRequestCommand`

Command for declining a friend request.

**Properties:**
- `string UserId` (get; set;): ID of the user declining the request
- `string FriendId` (get; set;): ID of the user whose request is being declined

##### `GetFriendRequestsQuery`

Query for retrieving pending friend requests.

**Properties:**
- `string UserId` (get; set;): ID of the user to get requests for

#### Friend Management Models

##### `GetFriendsListQuery`

Query for retrieving a user's friends list.

**Properties:**
- `string UserId` (get; set;): ID of the user to get friends for

##### `RemoveFriendCommand`

Command for removing a friend.

**Properties:**
- `string UserId` (get; set;): ID of the user removing the friend
- `string FriendId` (get; set;): ID of the friend to remove

### Response Models

#### `FriendRequestResponse`

Response model for friend request operations.

**Properties:**
- `bool Success` (get; set;): Whether the operation was successful
- `string Message` (get; set;): Response message
- `string RequestId` (get; set;): Unique identifier for the request

#### `FriendsListResponse`

Response model for friends list operations.

**Properties:**
- `List<FriendInfo> Friends` (get; set;): List of friends
- `int TotalCount` (get; set;): Total number of friends

#### `FriendInfo`

Information about a friend.

**Properties:**
- `string UserId` (get; set;): Friend's user ID
- `string Username` (get; set;): Friend's username
- `DateTime AddedDate` (get; set;): When the friendship was established
- `bool IsOnline` (get; set;): Whether the friend is currently online

## Configuration

### Environment Variables

- `AWS_REGION`: AWS region for service deployment
- `DYNAMODB_TABLE`: DynamoDB table for friend data storage
- `LOG_LEVEL`: Logging level (default: Information)

### AWS Systems Manager Parameters

- `/flyingdarts/friends/dynamodb-table`: DynamoDB table name
- `/flyingdarts/friends/region`: AWS region
- `/flyingdarts/friends/log-level`: Logging level

### Dependencies

**Internal Dependencies:**
- `Flyingdarts.Core`: Core business logic and extensions
- `Flyingdarts.Persistence`: Data persistence layer
- `Flyingdarts.DynamoDb.Service`: DynamoDB data access

**External Dependencies:**
- `Amazon.Lambda.APIGatewayEvents`: API Gateway event types
- `Amazon.Lambda.Core`: Lambda core functionality
- `Amazon.Lambda.RuntimeSupport`: Lambda runtime support
- `Amazon.Lambda.Serialization.SystemTextJson`: JSON serialization
- `Amazon.Extensions.Configuration.SystemsManager`: AWS Systems Manager
- `AWSSDK.ApiGatewayManagementApi`: API Gateway Management API
- `AWSSDK.DynamoDBv2`: DynamoDB SDK
- `AWSSDK.SimpleSystemsManagement`: AWS Systems Manager
- `FluentValidation`: Request validation
- `MediatR`: Mediator pattern implementation
- `Microsoft.Extensions.Configuration`: Configuration management
- `Microsoft.Extensions.DependencyInjection`: Dependency injection
- `Microsoft.Extensions.Options`: Options pattern

## Development

### Project Structure

```
friends/
├── Function.cs                    # Lambda entry point
├── InnerHandler.cs                # Inner request handler
├── ServiceFactory.cs              # Dependency injection setup
├── GlobalUsings.cs                # Global using statements
├── Models/                        # Data models
│   ├── Commands/                  # Command models
│   ├── Queries/                   # Query models
│   └── Responses/                 # Response models
├── Services/                      # Business logic services
│   ├── Handlers/                  # MediatR handlers
│   └── Validators/                # FluentValidation validators
├── Requests/                      # Request processing
├── Flyingdarts.Backend.Friends.Api.csproj  # Project file
└── README.md                      # This documentation
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

## Data Models

### DynamoDB Schema

#### Friends Table

```json
{
  "TableName": "flyingdarts-friends",
  "KeySchema": [
    {
      "AttributeName": "UserId",
      "KeyType": "HASH"
    },
    {
      "AttributeName": "FriendId",
      "KeyType": "RANGE"
    }
  ],
  "AttributeDefinitions": [
    {
      "AttributeName": "UserId",
      "AttributeType": "S"
    },
    {
      "AttributeName": "FriendId",
      "AttributeType": "S"
    }
  ]
}
```

#### Friend Request Table

```json
{
  "TableName": "flyingdarts-friend-requests",
  "KeySchema": [
    {
      "AttributeName": "RequestId",
      "KeyType": "HASH"
    }
  ],
  "AttributeDefinitions": [
    {
      "AttributeName": "RequestId",
      "AttributeType": "S"
    }
  ],
  "GlobalSecondaryIndexes": [
    {
      "IndexName": "UserIdIndex",
      "KeySchema": [
        {
          "AttributeName": "UserId",
          "KeyType": "HASH"
        }
      ],
      "Projection": {
        "ProjectionType": "ALL"
      }
    }
  ]
}
```

## Business Logic

### Friend Request Workflow

1. **Send Request**: User A sends friend request to User B
2. **Request Storage**: Request stored in DynamoDB with pending status
3. **Notification**: User B receives notification of friend request
4. **Response**: User B accepts or declines the request
5. **Update**: Friend relationship updated based on response

### Friend Management

- **Adding Friends**: Only through mutual friend request acceptance
- **Removing Friends**: Either user can remove the friendship
- **Friend Lists**: Real-time list of current friends
- **Online Status**: Integration with presence service for online status

## Security Considerations

### Authentication

- **JWT Validation**: All requests require valid JWT tokens
- **User Authorization**: Users can only access their own friend data
- **Request Validation**: All input data is validated and sanitized

### Data Privacy

- **Friend Data**: Friend relationships are private between users
- **Request Privacy**: Friend requests are only visible to involved users
- **Audit Logging**: All friend operations are logged for security

## Performance Considerations

### DynamoDB Optimization

- **Partition Keys**: Efficient partition key design for user data
- **Global Secondary Indexes**: Optimized queries for friend requests
- **Consistent Reads**: Use consistent reads for critical operations
- **Batch Operations**: Batch friend list operations for efficiency

### Caching

- **Friend Lists**: Consider caching frequently accessed friend lists
- **Request Status**: Cache friend request status to reduce DynamoDB calls
- **User Profiles**: Cache user profile information for friend lists

## Monitoring and Logging

### CloudWatch Logs

Monitor friend operations:

```bash
aws logs filter-log-events --log-group-name /aws/lambda/flyingdarts-friends-api
```

### Key Metrics

- Friend request success/failure rates
- Friend list retrieval performance
- DynamoDB operation latency
- API response times

### Alerts

Set up CloudWatch alarms for:
- High error rates
- Increased latency
- DynamoDB throttling
- Unauthorized access attempts

## Troubleshooting

### Common Issues

1. **Friend Request Failures**: Check DynamoDB permissions and table configuration
2. **Duplicate Requests**: Implement idempotency for friend requests
3. **Performance Issues**: Monitor DynamoDB capacity and optimize queries
4. **Authentication Errors**: Verify JWT token validation

### Debugging

Enable detailed logging:

```bash
export LOG_LEVEL=Debug
```

Check CloudWatch logs for detailed execution information.

## Related Projects

- **flyingdarts-auth**: Authentication service for user validation
- **flyingdarts-x01-api**: Game API for playing with friends
- **flyingdarts-signalling**: Real-time communication for friend status
- **flyingdarts-persistence**: Data persistence layer

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow MediatR patterns for new commands/queries

## License

Part of the Flyingdarts Turbo monorepo. See root LICENSE file for details.
