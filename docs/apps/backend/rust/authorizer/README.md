# Flyingdarts Authorizer (Rust)

## Overview

The Flyingdarts Authorizer is a Rust-based AWS Lambda function that provides custom authorization for the Flying Darts Turbo platform. This service acts as an API Gateway custom authorizer, validating JWT tokens and determining user permissions for accessing protected resources.

This service is responsible for:
- Validating JWT tokens from Authress authentication service
- Implementing custom authorization logic for API Gateway
- Managing user permissions and access control
- Providing secure authentication for all Flying Darts services
- Integrating with the Rust auth package for token validation

## Features

- **JWT Token Validation**: Secure validation of Authress JWT tokens
- **Custom API Gateway Authorizer**: AWS Lambda authorizer for API Gateway
- **Rust Performance**: High-performance authorization with minimal latency
- **Authress Integration**: Native integration with Authress authentication service
- **Permission-Based Access Control**: Granular permission checking
- **AWS Lambda Optimization**: Optimized for serverless deployment
- **Security Best Practices**: Implements OAuth 2.0 and JWT standards
- **Error Handling**: Comprehensive error handling with proper HTTP responses
- **Async/Await Support**: Modern Rust async/await patterns

## Prerequisites

- Rust toolchain (latest stable version)
- AWS CLI configured with appropriate permissions
- Authress account and configuration
- Cargo package manager
- Docker (for containerized deployment)

## Installation

1. Clone the monorepo and navigate to the authorizer:
```bash
cd apps/backend/rust/authorizer
```

2. Build the project:
```bash
cargo build
```

3. Run tests:
```bash
cargo test
```

4. Build for release:
```bash
cargo build --release
```

## Usage

### Local Development

Run the authorizer locally for development:

```bash
cargo run
```

### AWS Lambda Deployment

1. Build for AWS Lambda:
```bash
cargo build --release --target x86_64-unknown-linux-musl
```

2. Create deployment package:
```bash
zip -j bootstrap.zip target/x86_64-unknown-linux-musl/release/bootstrap
```

3. Deploy using AWS CLI:
```bash
aws lambda update-function-code --function-name flyingdarts-rust-authorizer --zip-file fileb://bootstrap.zip
```

### API Gateway Integration

The authorizer is configured as a custom authorizer in API Gateway:

1. **Token Source**: Authorization header
2. **Identity Source**: `method.request.header.Authorization`
3. **Result TTL**: 300 seconds (5 minutes)
4. **Authorization Type**: Custom

## API Reference

### Main Modules

#### `lib.rs`

The main library module containing the authorization logic.

**Functions:**

##### `handler(event: ApiGatewayCustomAuthorizerRequest, _ctx: Context) -> Result<ApiGatewayCustomAuthorizerResponse, Error>`

Main handler for API Gateway custom authorizer requests.

**Parameters:**
- `event` (ApiGatewayCustomAuthorizerRequest): The authorization request from API Gateway
- `_ctx` (Context): Lambda execution context

**Returns:**
- `Result<ApiGatewayCustomAuthorizerResponse, Error>`: Authorization response with policy document

**Errors:**
- `Error::Unauthorized`: When token validation fails
- `Error::InvalidRequest`: When request is malformed

##### `generate_policy(principal_id: &str, effect: &str, resource: &str, context: Option<HashMap<String, serde_json::Value>>) -> ApiGatewayCustomAuthorizerResponse`

Generates an IAM policy document for API Gateway authorization.

**Parameters:**
- `principal_id` (&str): User identifier from the token
- `effect` (&str): Policy effect (Allow/Deny)
- `resource` (&str): API Gateway resource ARN
- `context` (Option<HashMap<String, serde_json::Value>>): Additional context information

**Returns:**
- `ApiGatewayCustomAuthorizerResponse`: Complete authorization response

##### `extract_token_from_header(authorization_header: &str) -> Result<String, Error>`

Extracts the JWT token from the Authorization header.

**Parameters:**
- `authorization_header` (&str): Full Authorization header value

**Returns:**
- `Result<String, Error>`: Extracted JWT token

**Errors:**
- `Error::InvalidRequest`: When Authorization header is malformed

#### `main.rs`

The main binary entry point for the Lambda function.

**Functions:**

##### `main() -> Result<(), Error>`

Main entry point that initializes the Lambda runtime and starts the handler.

**Returns:**
- `Result<(), Error>`: Success or error result

### Error Types

#### `Error`

Custom error type for authorization failures.

**Variants:**
- `Unauthorized`: Token validation failed
- `InvalidRequest`: Request is malformed or invalid
- `InternalError`: Internal server error
- `TokenExtractionError`: Failed to extract token from header

### Data Structures

#### `ApiGatewayCustomAuthorizerRequest`

AWS Lambda event structure for API Gateway custom authorizer requests.

**Fields:**
- `type_` (String): Event type
- `method_arn` (String): API Gateway method ARN
- `authorization_token` (String): Authorization token from header
- `headers` (Option<HashMap<String, String>>): Request headers

#### `ApiGatewayCustomAuthorizerResponse`

AWS Lambda response structure for API Gateway custom authorizer responses.

**Fields:**
- `principal_id` (String): User identifier
- `policy_document` (PolicyDocument): IAM policy document
- `context` (Option<HashMap<String, serde_json::Value>>): Additional context
- `usage_identifier_key` (Option<String>): Usage identifier

#### `PolicyDocument`

IAM policy document structure.

**Fields:**
- `version` (String): Policy version
- `statement` (Vec<Statement>): Policy statements

#### `Statement`

IAM policy statement structure.

**Fields:**
- `effect` (String): Policy effect (Allow/Deny)
- `action` (Vec<String>): Allowed actions
- `resource` (Vec<String>): Resource ARNs

## Configuration

### Environment Variables

- `AUTHRESS_SERVICE_URL`: Authress service URL
- `AUTHRESS_APPLICATION_ID`: Authress application identifier
- `AWS_REGION`: AWS region for service deployment
- `RUST_LOG`: Logging level (default: info)

### AWS Systems Manager Parameters

- `/flyingdarts/auth/authress-service-url`: Authress service URL
- `/flyingdarts/auth/authress-application-id`: Authress application ID
- `/flyingdarts/auth/jwt-issuer`: Expected JWT issuer
- `/flyingdarts/auth/jwt-audience`: Expected JWT audience

### Dependencies

**Internal Dependencies:**
- `flyingdarts-auth`: Rust auth package for token validation

**External Dependencies:**
- `lambda_runtime`: AWS Lambda runtime for Rust
- `tokio`: Async runtime for Rust
- `serde`: Serialization/deserialization
- `serde_json`: JSON handling
- `tracing`: Structured logging
- `tracing-subscriber`: Logging subscriber
- `aws_lambda_events`: AWS Lambda event types
- `http`: HTTP types and utilities

## Development

### Project Structure

```
authorizer/
├── src/
│   ├── lib.rs                     # Main library module
│   └── main.rs                    # Binary entry point
├── examples/
│   └── test_auth.rs               # Example usage
├── Cargo.toml                     # Project configuration
├── Cargo.lock                     # Dependency lock file
└── README.md                      # This documentation
```

### Testing

Run the test suite:

```bash
cargo test
```

Run with verbose output:

```bash
cargo test --verbose
```

Run specific tests:

```bash
cargo test test_token_validation
```

### Building

Build for development:

```bash
cargo build
```

Build for production:

```bash
cargo build --release
```

Build for AWS Lambda:

```bash
cargo build --release --target x86_64-unknown-linux-musl
```

### Code Quality

The project uses:
- **Clippy**: Rust linter for code quality
- **Rustfmt**: Code formatting
- **Cargo audit**: Security vulnerability scanning
- **Tracing**: Structured logging

Run quality checks:

```bash
cargo clippy
cargo fmt --check
cargo audit
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
4. **Token Validation**: Authorizer validates token with Authress
5. **Policy Generation**: Generates IAM policy based on validation result
6. **Access Control**: API Gateway enforces the generated policy

### Configuration

Configure Authress integration:

```toml
[dependencies]
flyingdarts-auth = { path = "../../../../packages/backend/rust/auth" }
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

### Rust Optimizations

- **Zero-Cost Abstractions**: Leverage Rust's zero-cost abstractions
- **Memory Safety**: No runtime overhead for memory safety
- **Async/Await**: Efficient async/await patterns for I/O operations
- **Minimal Dependencies**: Keep dependency tree minimal

### Lambda Optimization

- **Cold Start**: Minimize cold start time with optimized binary size
- **Memory Usage**: Configure appropriate memory allocation
- **Timeout**: Set appropriate Lambda timeout for token validation
- **Concurrency**: Monitor concurrent authorization requests

### Caching

- **Token Validation**: Consider caching validation results
- **Policy Generation**: Cache generated policies for similar requests
- **Authress Responses**: Cache Authress service responses

## Monitoring and Logging

### CloudWatch Logs

Monitor authorization requests:

```bash
aws logs filter-log-events --log-group-name /aws/lambda/flyingdarts-rust-authorizer
```

### Structured Logging

The service uses structured logging with tracing:

```rust
tracing::info!("Processing authorization request", user_id = &user_id);
tracing::error!("Token validation failed", error = %error);
```

### Key Metrics

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
5. **Build Issues**: Ensure proper target for Lambda deployment

### Debugging

Enable detailed logging:

```bash
export RUST_LOG=debug
```

Check CloudWatch logs for detailed execution information.

### Error Codes

- **401 Unauthorized**: Invalid or missing token
- **403 Forbidden**: Valid token but insufficient permissions
- **500 Internal Server Error**: Service configuration or network issues

### Build Issues

Common build issues and solutions:

1. **Musl Target**: Install musl target for Lambda deployment
   ```bash
   rustup target add x86_64-unknown-linux-musl
   ```

2. **Cross-Compilation**: Use Docker for consistent builds
   ```bash
   docker run --rm -v "$PWD":/code -w /code rust:latest cargo build --release --target x86_64-unknown-linux-musl
   ```

## Examples

### Basic Usage

```rust
use flyingdarts_authorizer::handler;
use aws_lambda_events::apigw::ApiGatewayCustomAuthorizerRequest;
use lambda_runtime::Context;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let request = ApiGatewayCustomAuthorizerRequest {
        type_: "TOKEN".to_string(),
        method_arn: "arn:aws:execute-api:region:account:api-id/stage/GET/resource".to_string(),
        authorization_token: "Bearer your-jwt-token".to_string(),
        headers: None,
    };
    
    let ctx = Context::default();
    let response = handler(request, ctx).await?;
    
    println!("Authorization response: {:?}", response);
    Ok(())
}
```

### Custom Error Handling

```rust
use flyingdarts_authorizer::Error;

fn handle_authorization_error(error: Error) {
    match error {
        Error::Unauthorized => {
            tracing::warn!("Unauthorized access attempt");
        }
        Error::InvalidRequest => {
            tracing::error!("Invalid request format");
        }
        Error::InternalError => {
            tracing::error!("Internal server error");
        }
        Error::TokenExtractionError => {
            tracing::warn!("Failed to extract token from header");
        }
    }
}
```

## Related Projects

- **flyingdarts-auth**: Rust auth package for token validation
- **flyingdarts-x01-api**: Game API (protected by this authorizer)
- **flyingdarts-friends-api**: Friends management API
- **flyingdarts-signalling**: Real-time communication service

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow Rust best practices and security guidelines

## License

Part of the Flying Darts Turbo monorepo. See root LICENSE file for details.