# Flyingdarts.Backend.Auth

## Overview

The Flyingdarts.Backend.Auth service is a .NET 8 AWS Lambda function that provides custom authorization for the Flyingdarts Turbo platform. This service acts as an API Gateway custom authorizer, validating JWT tokens and determining user permissions for accessing protected resources.

This service is responsible for:
- Validating JWT tokens from Authress authentication service
- Implementing custom authorization logic for API Gateway
- Managing user permissions and access control
- Providing secure authentication for all Flyingdarts services
- Integrating with Authress SDK for token validation

## Features

- **JWT Token Validation**: Secure validation of Authress JWT tokens
- **Custom API Gateway Authorizer**: AWS Lambda authorizer for API Gateway
- **Authress Integration**: Native integration with Authress authentication service
- **Permission-Based Access Control**: Granular permission checking
- **AWS Lambda Optimization**: Optimized for serverless deployment
- **Security Best Practices**: Implements OAuth 2.0 and JWT standards
- **Error Handling**: Comprehensive error handling with proper HTTP responses
- **Configuration Management**: Flexible configuration through AWS Systems Manager

## Prerequisites

- .NET 8 SDK
- AWS CLI configured with appropriate permissions
- Authress account and configuration
- Visual Studio 2022 or VS Code with C# extensions
- Docker (for containerized deployment)

## Installation

1. Clone the monorepo and navigate to the auth service:
```bash
cd apps/backend/dotnet/auth
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

Run the auth service locally for development:

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
aws lambda update-function-code --function-name flyingdarts-auth --zip-file fileb://Flyingdarts.Backend.Auth.zip
```

### API Gateway Integration

The auth service is configured as a custom authorizer in API Gateway:

1. **Token Source**: Authorization header
2. **Identity Source**: `method.request.header.Authorization`
3. **Result TTL**: 300 seconds (5 minutes)
4. **Authorization Type**: Custom

## API Reference

### Main Classes

#### `Function`

The main Lambda function entry point that handles API Gateway authorization requests.

**Properties:**
- `ILogger<Function> _logger`: Logging instance for the function
- `IAuthressService _authressService`: Authress service for token validation
- `IConfiguration _configuration`: Configuration management

**Methods:**

##### `FunctionHandler(APIGatewayCustomAuthorizerRequest request, ILambdaContext context): Task<APIGatewayCustomAuthorizerResponse>`

Main handler for API Gateway custom authorizer requests.

**Parameters:**
- `request` (APIGatewayCustomAuthorizerRequest): The authorization request from API Gateway
- `context` (ILambdaContext): Lambda execution context

**Returns:**
- `Task<APIGatewayCustomAuthorizerResponse>`: Authorization response with policy document

**Throws:**
- `UnauthorizedAccessException`: When token validation fails
- `ArgumentException`: When request is malformed

##### `GeneratePolicy(string principalId, string effect, string resource, Dictionary<string, object> context): APIGatewayCustomAuthorizerResponse`

Generates an IAM policy document for API Gateway authorization.

**Parameters:**
- `principalId` (string): User identifier from the token
- `effect` (string): Policy effect (Allow/Deny)
- `resource` (string): API Gateway resource ARN
- `context` (Dictionary<string, object>): Additional context information

**Returns:**
- `APIGatewayCustomAuthorizerResponse`: Complete authorization response

#### `IAuthressService`

Interface for Authress token validation service.

**Methods:**

##### `ValidateTokenAsync(string token): Task<AuthressValidationResult>`

Validates a JWT token using Authress service.

**Parameters:**
- `token` (string): JWT token to validate

**Returns:**
- `Task<AuthressValidationResult>`: Validation result with user information

**Throws:**
- `AuthressException`: When Authress service is unavailable
- `InvalidTokenException`: When token is invalid or expired

#### `AuthressValidationResult`

Result of Authress token validation.

**Properties:**
- `bool IsValid` (get; set;): Whether the token is valid
- `string UserId` (get; set;): User identifier from the token
- `List<string> Permissions` (get; set;): User permissions
- `Dictionary<string, object> Claims` (get; set;): Additional token claims

#### `JWT`

Utility class for JWT token handling.

**Methods:**

##### `ExtractTokenFromAuthorizationHeader(string authorizationHeader): string`

Extracts the JWT token from the Authorization header.

**Parameters:**
- `authorizationHeader` (string): Full Authorization header value

**Returns:**
- `string`: Extracted JWT token

**Throws:**
- `ArgumentException`: When Authorization header is malformed

#### `APIGatewayCustomAuthorizerRequestExtensions`

Extension methods for API Gateway custom authorizer requests.

**Methods:**

##### `GetAuthorizationHeader(this APIGatewayCustomAuthorizerRequest request): string`

Gets the Authorization header from the request.

**Parameters:**
- `request` (APIGatewayCustomAuthorizerRequest): The authorization request

**Returns:**
- `string`: Authorization header value

**Throws:**
- `ArgumentException`: When Authorization header is missing

## Configuration

### Environment Variables

- `AUTHRESS_SERVICE_URL`: Authress service URL
- `AUTHRESS_APPLICATION_ID`: Authress application identifier
- `AWS_REGION`: AWS region for service deployment
- `LOG_LEVEL`: Logging level (default: Information)

### AWS Systems Manager Parameters

- `/flyingdarts/auth/authress-service-url`: Authress service URL
- `/flyingdarts/auth/authress-application-id`: Authress application ID
- `/flyingdarts/auth/jwt-issuer`: Expected JWT issuer
- `/flyingdarts/auth/jwt-audience`: Expected JWT audience

### Dependencies

**External Dependencies:**
- `Amazon.Lambda.APIGatewayEvents`: API Gateway event types
- `Amazon.Lambda.Core`: Lambda core functionality
- `Amazon.Lambda.RuntimeSupport`: Lambda runtime support
- `Amazon.Lambda.Serialization.SystemTextJson`: JSON serialization
- `Authress.SDK`: Authress authentication SDK
- `AWSSDK.SimpleSystemsManagement`: AWS Systems Manager
- `Microsoft.Extensions.Configuration`: Configuration management
- `Microsoft.Extensions.DependencyInjection.Abstractions`: DI abstractions
- `Microsoft.Extensions.Options`: Options pattern
- `Microsoft.IdentityModel.Tokens`: JWT token handling
- `System.IdentityModel.Tokens.Jwt`: JWT validation

## Development

### Project Structure

```
auth/
├── Function.cs                                    # Lambda entry point
├── IAuthressService.cs                           # Authress service interface
├── AuthressService.cs                            # Authress service implementation
├── AuthressValidationResult.cs                   # Validation result model
├── JWT.cs                                        # JWT utility class
├── APIGatewayCustomAuthorizerRequestExtensions.cs # Request extensions
├── GlobalUsings.cs                               # Global using statements
├── Flyingdarts.Backend.Auth.csproj               # Project file
└── README.md                                     # This documentation
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
- **Microsoft Identity Model**: JWT validation
- **Authress SDK**: Authentication service integration
- **Dependency Injection**: Service composition

Run quality checks:

```bash
dotnet build --verbosity normal
```

## Security Considerations

### Token Validation

- **JWT Standards**: Implements RFC 7519 JWT standards
- **Token Expiration**: Validates token expiration time
- **Signature Verification**: Verifies JWT signature using Authress public keys
- **Issuer Validation**: Validates token issuer against configured value
- **Audience Validation**: Validates token audience

### Access Control

- **Principle of Least Privilege**: Grants minimum required permissions
- **Resource-Based Policies**: Uses IAM policies for resource access
- **Permission Validation**: Validates user permissions from token claims
- **Context Passing**: Passes user context to downstream services

### Error Handling

- **Secure Error Messages**: Avoids leaking sensitive information
- **Proper HTTP Status Codes**: Returns appropriate authorization responses
- **Logging**: Comprehensive logging for security auditing
- **Exception Handling**: Graceful handling of validation failures

## Integration with Authress

### Authentication Flow

1. **User Login**: User authenticates through Authress
2. **Token Issuance**: Authress issues JWT token
3. **API Request**: Client includes token in Authorization header
4. **Token Validation**: Auth service validates token with Authress
5. **Policy Generation**: Generates IAM policy based on validation result
6. **Access Control**: API Gateway enforces the generated policy

### Configuration

Configure Authress integration:

```json
{
  "Authress": {
    "ServiceUrl": "https://your-domain.authress.io",
    "ApplicationId": "your-application-id"
  }
}
```

### Token Format

Expected JWT token format:

```json
{
  "iss": "https://your-domain.authress.io",
  "aud": "your-application-id",
  "sub": "user-id",
  "exp": 1234567890,
  "iat": 1234567890,
  "permissions": ["read:games", "write:scores"]
}
```

## Performance Considerations

### Caching

- **Token Validation**: Consider caching validation results
- **Policy Generation**: Cache generated policies for similar requests
- **Authress Responses**: Cache Authress service responses

### Optimization

- **Cold Start**: Minimize cold start time with optimized dependencies
- **Memory Usage**: Configure appropriate memory allocation
- **Timeout**: Set appropriate Lambda timeout for token validation
- **Concurrency**: Monitor concurrent authorization requests

## Monitoring and Logging

### CloudWatch Logs

Monitor authorization requests:

```bash
aws logs filter-log-events --log-group-name /aws/lambda/flyingdarts-auth
```

### Metrics

Key metrics to monitor:
- Authorization success/failure rates
- Token validation latency
- Authress service response times
- Error rates by error type

### Alerts

Set up CloudWatch alarms for:
- High error rates
- Increased latency
- Authress service failures
- Unauthorized access attempts

## Troubleshooting

### Common Issues

1. **Token Validation Failures**: Check Authress configuration and token format
2. **Permission Denied**: Verify user permissions in Authress
3. **Configuration Errors**: Validate AWS Systems Manager parameters
4. **Network Issues**: Check connectivity to Authress service

### Debugging

Enable detailed logging:

```bash
export LOG_LEVEL=Debug
```

Check CloudWatch logs for detailed execution information.

### Error Codes

- **401 Unauthorized**: Invalid or missing token
- **403 Forbidden**: Valid token but insufficient permissions
- **500 Internal Server Error**: Service configuration or network issues

## Related Projects

- **flyingdarts-x01-api**: X01 game API (protected by this auth service)
- **flyingdarts-friends-api**: Friends management API
- **flyingdarts-signalling**: Real-time communication service
- **authress-flutter-package**: Flutter authentication package

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow security best practices for authentication code

## License

Part of the Flyingdarts Turbo monorepo. See root LICENSE file for details.
