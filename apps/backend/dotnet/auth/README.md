# Flyingdarts Auth API

## Overview

The Flyingdarts Auth API is a .NET 8 AWS Lambda function that serves as a custom authorizer for API Gateway. It provides authentication and authorization services for the Flyingdarts Turbo platform by validating tokens through Authress integration and generating appropriate IAM policies for AWS resource access.

This service is responsible for:
- Validating authentication tokens from API Gateway requests
- Integrating with Authress for enterprise authentication
- Generating IAM policies for AWS resource access control
- Supporting both HTTP and WebSocket authentication flows
- Providing detailed logging for debugging and monitoring

## Features

- **API Gateway Custom Authorizer**: Handles authentication for API Gateway endpoints
- **Authress Integration**: Enterprise-grade authentication via Authress service
- **Token Validation**: Comprehensive JWT token validation and verification
- **IAM Policy Generation**: Dynamic IAM policy creation based on authentication results
- **WebSocket Support**: Authentication for WebSocket connections via query parameters
- **HTTP Support**: Standard HTTP authentication via Authorization headers
- **Comprehensive Logging**: Detailed logging for debugging and monitoring
- **Error Handling**: Robust error handling with proper IAM policy responses

## Prerequisites

- .NET 8 SDK
- AWS CLI configured with appropriate permissions
- Authress account and configuration
- Visual Studio 2022 or VS Code with C# extensions

## Installation

1. Clone the monorepo and navigate to the auth API:
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

Run the auth API locally for development:

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

Configure the Lambda function as an API Gateway custom authorizer:

```yaml
# Example CloudFormation configuration
AuthFunction:
  Type: AWS::Lambda::Function
  Properties:
    FunctionName: flyingdarts-auth
    Runtime: dotnet8
    Handler: Flyingdarts.Backend.Auth::Flyingdarts.Backend.Auth.Function::FunctionHandler
    Code:
      ZipFile: |
        # Lambda deployment package

ApiGatewayAuthorizer:
  Type: AWS::ApiGateway::Authorizer
  Properties:
    Name: FlyingDartsAuthorizer
    Type: TOKEN
    AuthorizerUri: !Sub "arn:aws:apigateway:${AWS::Region}:lambda:path/2015-03-31/functions/${AuthFunction.Arn}/invocations"
    AuthorizerResultTtlInSeconds: 300
```

## API Reference

### Main Function

#### `FunctionHandler(APIGatewayCustomAuthorizerRequest apiGatewayEvent, ILambdaContext context): Task<APIGatewayCustomAuthorizerResponse>`

The primary Lambda handler for API Gateway custom authorization requests.

**Parameters:**
- `apiGatewayEvent` (APIGatewayCustomAuthorizerRequest): The incoming API Gateway authorization request
- `context` (ILambdaContext): Lambda execution context

**Returns:**
- `APIGatewayCustomAuthorizerResponse`: Authorization response with IAM policy

**Example:**
```csharp
var response = await handler(apiGatewayEvent, context);
```

### Helper Functions

#### `ExtractToken(): string`

Extracts the authentication token from the request.

**Token Sources:**
1. **WebSocket Connections**: Extracts from query string `token` parameter when ConnectionId is present
2. **HTTP Requests**: Extracts from `Authorization` header (supports "Bearer " prefix)

**Returns:**
- `string`: Extracted token or null if not found

**Throws:**
- No exceptions thrown, returns null for missing tokens

#### `ValidateToken(string token): Task<string>`

Validates the authentication token using Authress service.

**Parameters:**
- `token` (string): The authentication token to validate

**Returns:**
- `string`: User ID from validated token

**Throws:**
- `ArgumentException`: When token is null or empty
- `InvalidOperationException`: When Authress configuration is missing
- `Exception`: When Authress validation fails

### Data Structures

#### `JWT`

JWT token structure for token validation.

**Properties:**
- `iss` (string): Token issuer
- `sub` (string): Token subject (user ID)
- `iat` (int): Issued at timestamp
- `exp` (int): Expiration timestamp
- `scope` (string): Token scope/permissions
- `azp` (string): Authorized party
- `client_id` (string): Client identifier

#### `APIGatewayCustomAuthorizerRequest`

API Gateway custom authorizer request structure.

**Properties:**
- `MethodArn` (string): ARN of the API Gateway method being accessed
- `Headers` (Dictionary<string, string>): Request headers
- `QueryStringParameters` (Dictionary<string, string>): Query string parameters
- `RequestContext` (APIGatewayCustomAuthorizerRequestContext): Request context information

#### `APIGatewayCustomAuthorizerResponse`

API Gateway custom authorizer response structure.

**Properties:**
- `PrincipalID` (string): Identifier for the requesting principal
- `PolicyDocument` (APIGatewayCustomAuthorizerPolicy): IAM policy document
- `Context` (APIGatewayCustomAuthorizerContextOutput): Additional context information

## Configuration

### Environment Variables

- `AuthressApiBasePath`: Authress service base URL (required)
- `AWS_REGION`: AWS region for service deployment
- `LOG_LEVEL`: Logging level (default: Information)

### Dependencies

**External Dependencies:**
- `Amazon.Lambda.APIGatewayEvents`: API Gateway event types
- `Amazon.Lambda.Core`: Lambda core functionality
- `Amazon.Lambda.RuntimeSupport`: Lambda runtime support
- `Amazon.Lambda.Serialization.SystemTextJson`: JSON serialization
- `Authress.SDK`: Authress authentication SDK
- `AWSSDK.SimpleSystemsManagement`: AWS Systems Manager for configuration
- `Microsoft.Extensions.Configuration`: Configuration management
- `Microsoft.Extensions.DependencyInjection.Abstractions`: Dependency injection
- `Microsoft.Extensions.Options`: Options pattern support
- `Microsoft.IdentityModel.Tokens`: JWT token handling
- `System.IdentityModel.Tokens.Jwt`: JWT token validation

## Development

### Project Structure

```
auth/
├── Function.cs                                    # Main Lambda handler
├── JWT.cs                                        # JWT token structure
├── APIGatewayCustomAuthorizerRequestExtensions.cs # Request extensions
├── GlobalUsings.cs                               # Global using statements
├── Flyingdarts.Backend.Auth.csproj              # Project file
├── aws-lambda-tools-defaults.json               # Lambda deployment config
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
- **Nullable Reference Types**: Null safety enforcement
- **Implicit Usings**: Simplified using statements
- **Latest C# Features**: Modern C# language features

Run quality checks:

```bash
dotnet build --verbosity normal
```

## Authentication Flow

### HTTP Request Flow

1. **Request Received**: API Gateway receives HTTP request
2. **Token Extraction**: Extract token from `Authorization` header
3. **Token Validation**: Validate token with Authress service
4. **Policy Generation**: Generate IAM policy based on validation result
5. **Response**: Return authorization response to API Gateway

### WebSocket Connection Flow

1. **Connection Request**: API Gateway receives WebSocket connection request
2. **Token Extraction**: Extract token from query string `token` parameter
3. **Token Validation**: Validate token with Authress service
4. **Policy Generation**: Generate IAM policy for WebSocket access
5. **Connection Authorization**: Authorize or deny WebSocket connection

### Token Validation Process

1. **Configuration Check**: Verify Authress configuration is present
2. **Client Creation**: Create Authress client with token provider
3. **Token Verification**: Call Authress `VerifyToken` method
4. **User Identity**: Extract user identity from validated token
5. **Error Handling**: Handle validation errors with appropriate responses

## Related Projects

- **flyingdarts-authorizer**: Rust-based authorizer for comparison
- **flyingdarts-x01-api**: Games API that uses this authorizer
- **flyingdarts-signalling-api**: Signalling API for WebSocket management
- **flyingdarts-friends-api**: Friends API with authentication

## Troubleshooting

### Common Issues

1. **Missing Authress Configuration**: Ensure `AuthressApiBasePath` environment variable is set
2. **Token Extraction Failures**: Check token format in Authorization header or query string
3. **Authress Validation Errors**: Verify Authress service connectivity and token validity
4. **IAM Policy Generation**: Ensure proper IAM permissions for API Gateway

### Debugging

Enable detailed logging by checking CloudWatch logs for the Lambda function. The function provides comprehensive logging with `[AUTH]` prefixes.

Common log messages:
- `[AUTH] Starting authorization for request`
- `[AUTH] Extracting token...`
- `[AUTH] Starting token validation...`
- `[AUTH] Authorization successful`

### Performance

- **Memory**: Configure appropriate memory allocation (256MB minimum)
- **Timeout**: Set timeout based on Authress response times (30 seconds recommended)
- **Concurrency**: Monitor concurrent authorization requests
- **Caching**: Consider API Gateway authorizer result caching

## Security Considerations

- **Token Security**: Always use HTTPS/WSS in production
- **Token Validation**: Validate tokens with Authress before processing
- **IAM Policies**: Follow principle of least privilege when generating policies
- **Error Messages**: Avoid exposing sensitive information in error responses
- **Logging**: Ensure sensitive data is not logged in production
- **Token Storage**: Never store tokens in logs or error messages

### Best Practices

1. **Use Strong Tokens**: Ensure Authress generates strong, time-limited tokens
2. **Validate All Requests**: Never skip token validation for any endpoint
3. **Monitor Access**: Use CloudWatch to monitor authorization patterns
4. **Regular Updates**: Keep Authress SDK and dependencies updated
5. **Security Audits**: Regularly audit IAM policies and access patterns

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow security best practices for authentication code

## License

Part of the Flyingdarts Turbo monorepo. See root LICENSE file for details.
