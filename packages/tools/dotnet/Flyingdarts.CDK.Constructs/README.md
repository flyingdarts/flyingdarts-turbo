# Flyingdarts.CDK.Constructs

## Overview

The Flyingdarts.CDK.Constructs package provides reusable AWS CDK constructs for the Flying Darts Turbo platform infrastructure. This package contains pre-built, tested, and production-ready CDK constructs that simplify the deployment and management of Flying Darts services on AWS.

The package is designed to:
- Provide standardized AWS infrastructure components
- Simplify CDK stack creation and management
- Ensure consistent infrastructure across environments
- Support multiple AWS services integration
- Enable rapid infrastructure deployment
- Maintain security and best practices

## Features

- **API Gateway Constructs**: WebSocket and REST API configurations
- **Lambda Constructs**: Serverless function deployment and configuration
- **DynamoDB Constructs**: Database table creation and management
- **Authorization Constructs**: GitHub OAuth and custom authorizers
- **Stack Management**: Complete Flying Darts infrastructure stack
- **Aspects Integration**: Custom CDK aspects for cross-cutting concerns
- **Configuration Management**: Environment-specific configurations
- **Security Best Practices**: Built-in security configurations and IAM policies

## Prerequisites

- .NET 8.0 or later
- AWS CDK CLI (version 2.206.0 or higher)
- AWS credentials configured
- Constructs library (version 10.4.2 or higher)

## Installation

Add the package to your project file:

```xml
<ItemGroup>
  <PackageReference Include="Flyingdarts.CDK.Constructs" Version="1.0.2" />
</ItemGroup>
```

Or install via NuGet:

```bash
dotnet add package Flyingdarts.CDK.Constructs
```

## Usage

### Basic Stack Setup

```csharp
using Amazon.CDK;
using Flyingdarts.CDK.Constructs.v2;

var app = new App();

var stack = new FlyingdartsStack(app, "FlyingdartsStack", new StackProps
{
    Env = new Environment
    {
        Account = "123456789012",
        Region = "eu-west-1"
    }
});

app.Synth();
```

### API Gateway Configuration

```csharp
using Flyingdarts.CDK.Constructs.v2;

var apiGateway = new ApiGatewayConstruct(this, "ApiGateway", new ApiGatewayConstructProps
{
    StageName = "Development",
    CorsEnabled = true,
    WebSocketEnabled = true
});

// Add routes to the API Gateway
apiGateway.AddWebSocketRoute("$connect", "connect-handler");
apiGateway.AddWebSocketRoute("$disconnect", "disconnect-handler");
apiGateway.AddWebSocketRoute("message", "message-handler");
```

### Lambda Function Deployment

```csharp
using Flyingdarts.CDK.Constructs.v2;

var lambdaConstruct = new LambdaConstruct(this, "LambdaFunctions", new LambdaConstructProps
{
    Runtime = Runtime.DOTNET_8,
    Handler = "Flyingdarts.Lambda::Flyingdarts.Lambda.Function::FunctionHandler",
    Timeout = Duration.Seconds(30),
    MemorySize = 512
});

// Add individual Lambda functions
var connectHandler = lambdaConstruct.AddFunction("connect-handler", new LambdaFunctionProps
{
    Code = Code.FromAsset("lambda/connect-handler.zip"),
    Environment = new Dictionary<string, string>
    {
        ["ENVIRONMENT"] = "Development"
    }
});
```

### DynamoDB Table Creation

```csharp
using Flyingdarts.CDK.Constructs.v2;

var dynamoDb = new DynamoDbConstruct(this, "DynamoDb", new DynamoDbConstructProps
{
    TableName = "flyingdarts-users",
    PartitionKey = "UserId",
    SortKey = "Timestamp",
    BillingMode = BillingMode.PAY_PER_REQUEST
});

// Add GSI (Global Secondary Index)
dynamoDb.AddGlobalSecondaryIndex("ConnectionIndex", new GlobalSecondaryIndexProps
{
    IndexName = "ConnectionIndex",
    PartitionKey = "ConnectionId",
    ProjectionType = ProjectionType.ALL
});
```

### GitHub Authorization

```csharp
using Flyingdarts.CDK.Constructs.v2;

var githubAuth = new GithubAuthConstruct(this, "GithubAuth", new GithubAuthConstructProps
{
    ClientId = "your-github-client-id",
    ClientSecret = "your-github-client-secret",
    AllowedOrganizations = new[] { "flyingdarts" }
});

// Use the authorizer in API Gateway
apiGateway.AddAuthorizer("github-auth", githubAuth.Authorizer);
```

### Complete Infrastructure Example

```csharp
using Amazon.CDK;
using Flyingdarts.CDK.Constructs.v2;

public class FlyingdartsInfrastructureStack : Stack
{
    public FlyingdartsInfrastructureStack(Construct scope, string id, IStackProps props = null) 
        : base(scope, id, props)
    {
        // Create DynamoDB tables
        var usersTable = new DynamoDbConstruct(this, "UsersTable", new DynamoDbConstructProps
        {
            TableName = "flyingdarts-users",
            PartitionKey = "UserId"
        });

        var gamesTable = new DynamoDbConstruct(this, "GamesTable", new DynamoDbConstructProps
        {
            TableName = "flyingdarts-games",
            PartitionKey = "GameId",
            SortKey = "Timestamp"
        });

        // Create Lambda functions
        var lambdaConstruct = new LambdaConstruct(this, "LambdaFunctions", new LambdaConstructProps
        {
            Runtime = Runtime.DOTNET_8,
            Handler = "Flyingdarts.Lambda::Flyingdarts.Lambda.Function::FunctionHandler"
        });

        var connectHandler = lambdaConstruct.AddFunction("connect-handler");
        var disconnectHandler = lambdaConstruct.AddFunction("disconnect-handler");
        var messageHandler = lambdaConstruct.AddFunction("message-handler");

        // Create API Gateway
        var apiGateway = new ApiGatewayConstruct(this, "ApiGateway", new ApiGatewayConstructProps
        {
            StageName = "Development",
            WebSocketEnabled = true
        });

        // Add routes
        apiGateway.AddWebSocketRoute("$connect", connectHandler);
        apiGateway.AddWebSocketRoute("$disconnect", disconnectHandler);
        apiGateway.AddWebSocketRoute("message", messageHandler);

        // Grant permissions
        usersTable.GrantReadWriteData(connectHandler);
        usersTable.GrantReadWriteData(disconnectHandler);
        gamesTable.GrantReadWriteData(messageHandler);
    }
}
```

## API Reference

### Constructs

#### FlyingdartsStack

The main stack that orchestrates all Flying Darts infrastructure components.

**Properties:**
- `ApiGateway` (ApiGatewayConstruct): The API Gateway construct
- `LambdaFunctions` (LambdaConstruct): The Lambda functions construct
- `DynamoDbTables` (DynamoDbConstruct[]): Array of DynamoDB table constructs
- `Authorizers` (AuthorizersConstruct): The authorization constructs

#### ApiGatewayConstruct

Manages API Gateway configuration including WebSocket and REST APIs.

**Methods:**

##### AddWebSocketRoute(string, ILambdaFunction): void
Adds a WebSocket route to the API Gateway.

**Parameters:**
- `routeKey` (string): The route key (e.g., "$connect", "message")
- `handler` (ILambdaFunction): The Lambda function to handle the route

##### AddRestRoute(string, ILambdaFunction): void
Adds a REST API route to the API Gateway.

**Parameters:**
- `path` (string): The API path
- `handler` (ILambdaFunction): The Lambda function to handle the route

##### AddAuthorizer(string, IAuthorizer): void
Adds an authorizer to the API Gateway.

**Parameters:**
- `name` (string): The authorizer name
- `authorizer` (IAuthorizer): The authorizer instance

#### LambdaConstruct

Manages Lambda function deployment and configuration.

**Methods:**

##### AddFunction(string, LambdaFunctionProps): ILambdaFunction
Adds a new Lambda function.

**Parameters:**
- `name` (string): The function name
- `props` (LambdaFunctionProps): Function configuration properties

**Returns:**
- `ILambdaFunction`: The created Lambda function

#### DynamoDbConstruct

Manages DynamoDB table creation and configuration.

**Methods:**

##### AddGlobalSecondaryIndex(string, GlobalSecondaryIndexProps): void
Adds a Global Secondary Index to the table.

**Parameters:**
- `indexName` (string): The index name
- `props` (GlobalSecondaryIndexProps): Index configuration properties

##### GrantReadWriteData(IGrantable): Grant
Grants read/write permissions to the specified grantable.

**Parameters:**
- `grantable` (IGrantable): The resource to grant permissions to

**Returns:**
- `Grant`: The permission grant

#### GithubAuthConstruct

Manages GitHub OAuth authorization configuration.

**Properties:**
- `Authorizer` (IAuthorizer): The GitHub OAuth authorizer

#### AuthorizersConstruct

Manages multiple authorization methods.

**Properties:**
- `GithubAuth` (GithubAuthConstruct): GitHub OAuth authorization
- `CustomAuthorizers` (Dictionary<string, IAuthorizer>): Custom authorizers

### Aspects

The package includes custom CDK aspects for cross-cutting concerns:

#### SecurityAspect
Applies security best practices to all resources.

#### LoggingAspect
Configures CloudWatch logging for all resources.

#### TaggingAspect
Applies consistent tagging across all resources.

## Configuration

### Environment Variables

The constructs support the following environment variables:

- `ENVIRONMENT`: The deployment environment (Development, Staging, Production)
- `REGION`: The AWS region for deployment
- `ACCOUNT`: The AWS account ID

### CDK Context

Configure the constructs using CDK context:

```json
{
  "flyingdarts": {
    "environment": "Development",
    "region": "eu-west-1",
    "github": {
      "clientId": "your-client-id",
      "clientSecret": "your-client-secret",
      "allowedOrganizations": ["flyingdarts"]
    }
  }
}
```

## Development

### Building the Package

```bash
dotnet build Flyingdarts.CDK.Constructs.csproj
```

### Running Tests

```bash
dotnet test
```

### Deploying Infrastructure

```bash
cdk deploy FlyingdartsStack
```

### Synthesizing CloudFormation

```bash
cdk synth
```

### Code Style

The project follows .NET coding conventions with:
- XML documentation for all public APIs
- Nullable reference types enabled
- Implicit usings for cleaner code
- Consistent naming conventions
- Proper error handling and validation

## Dependencies

### Internal Dependencies
- None (this is a standalone CDK constructs package)

### External Dependencies
- **Amazon.CDK.Lib** (2.206.0): AWS CDK core library
- **Amazon.CDK.AWS.APIGatewayv2.Alpha** (2.103.1-alpha.0): API Gateway v2 constructs
- **Amazon.CDK.AWS.APIGatewayv2.Integrations.Alpha** (2.103.1-alpha.0): API Gateway integrations
- **Amazon.CDK.AWS.APIGatewayv2.Authorizers.Alpha** (2.103.1-alpha.0): API Gateway authorizers
- **Constructs** (10.4.2): CDK constructs library
- **AWSSDK.SimpleSystemsManagement** (3.7.406.7): AWS Systems Manager
- **Microsoft.Extensions.Configuration** (10.0.0-preview.6.25358.103): Configuration management
- **Microsoft.Extensions.DependencyInjection.Abstractions** (10.0.0-preview.6.25358.103): DI abstractions
- **Microsoft.Extensions.Options** (10.0.0-preview.6.25358.103): Options pattern

## Security

The constructs implement several security best practices:

- **IAM Least Privilege**: Minimal required permissions for all resources
- **VPC Configuration**: Optional VPC integration for Lambda functions
- **Encryption**: Default encryption for all data at rest
- **Network Security**: Security groups and network ACLs where applicable
- **Secret Management**: Integration with AWS Secrets Manager
- **Audit Logging**: CloudTrail and CloudWatch logging enabled

## Related Projects

- **Flyingdarts.Lambda.Core**: Lambda function infrastructure
- **Flyingdarts.Core**: Core models and utilities
- **Flyingdarts.Connection.Services**: WebSocket connection management
- **Flyingdarts.DynamoDb.Service**: DynamoDB service implementation

## Contributing

When contributing to this package:

1. Follow the existing code style and patterns
2. Add XML documentation for all public APIs
3. Include unit tests for new functionality
4. Test CDK synthesis and deployment
5. Update this README for any new features or breaking changes
6. Ensure security best practices are maintained
7. Test with multiple AWS regions and environments

## License

This package is part of the Flying Darts Turbo monorepo and follows the same licensing terms.