# Flying Darts Authorizer

## Overview

The Flying Darts Authorizer is a Rust-based AWS Lambda function that provides authorization services for the Flying Darts Turbo monorepo. It serves as an API Gateway authorizer and handles token validation using both custom authentication logic and Authress integration.

This service is responsible for:
- Validating authorization tokens for API Gateway requests
- Managing IAM policy generation for AWS resources
- Integrating with Authress for enterprise authentication
- Providing both custom and standard authentication flows

## Features

- **AWS Lambda Integration**: Built as a Lambda function for serverless deployment
- **API Gateway Authorizer**: Handles authorization for API Gateway endpoints
- **Authress Integration**: Supports enterprise authentication via Authress
- **Custom Authentication**: Includes example authentication service for testing
- **IAM Policy Generation**: Dynamically generates IAM policies based on authorization results
- **Structured Logging**: Comprehensive logging with tracing and structured output
- **Error Handling**: Robust error handling with proper HTTP status codes

## Prerequisites

- Rust 1.70+ with Cargo
- AWS CLI configured with appropriate permissions
- Authress account and configuration (for production use)
- Docker (for containerized deployment)

## Installation

1. Clone the monorepo and navigate to the authorizer:
```bash
cd apps/backend/rust/authorizer
```

2. Install dependencies:
```bash
cargo build
```

3. Run tests:
```bash
cargo test
```

## Usage

### Local Development

Run the authorizer locally for testing:

```bash
cargo run --example test_auth
```

### AWS Lambda Deployment

1. Build for AWS Lambda:
```bash
cargo build --release --target x86_64-unknown-linux-musl
```

2. Package and deploy using AWS CLI or CDK.

### API Gateway Integration

Configure the Lambda function as an API Gateway authorizer:

```yaml
# Example CloudFormation configuration
AuthorizerFunction:
  Type: AWS::Lambda::Function
  Properties:
    FunctionName: flyingdarts-authorizer
    Runtime: provided.al2
    Handler: bootstrap
    Code:
      ZipFile: |
        # Lambda deployment package
```

## API Reference

### Main Functions

#### `function_handler(event: LambdaEvent<Request>) -> Result<Response, Error>`

The primary Lambda handler for API Gateway authorization requests.

**Parameters:**
- `event`: Lambda event containing authorization request data

**Returns:**
- `Response`: IAM policy document with authorization decision

**Example:**
```rust
let response = function_handler(event).await?;
```

#### `authress_handler(event: LambdaEvent<ApiGatewayProxyRequest>) -> Result<ApiGatewayProxyResponse, Error>`

Handler for API Gateway proxy requests with Authress integration.

**Parameters:**
- `event`: API Gateway proxy request event

**Returns:**
- `ApiGatewayProxyResponse`: HTTP response with authorization result

### Data Structures

#### `Request`

Represents an authorization request from API Gateway.

**Properties:**
- `_type` (String): Request type identifier
- `method_arn` (String): ARN of the API Gateway method being accessed
- `authorization_token` (String): Authorization token to validate

#### `Response`

Represents the authorization response with IAM policy.

**Properties:**
- `principal_id` (String): Identifier for the requesting principal
- `policy_document` (PolicyDocument): IAM policy document
- `context` (serde_json::Value): Additional context information

#### `PolicyDocument`

IAM policy document structure.

**Properties:**
- `version` (String): Policy version (typically "2012-10-17")
- `statement` (Vec<Statement>): Array of policy statements

#### `Statement`

Individual IAM policy statement.

**Properties:**
- `action` (String): AWS action being authorized
- `effect` (String): Policy effect ("Allow" or "Deny")
- `resource` (String): AWS resource ARN

## Configuration

### Environment Variables

- `AUTHRESS_BASE_URL`: Authress service base URL
- `AUTHRESS_ACCESS_KEY`: Authress access key for API calls
- `LOG_LEVEL`: Logging level (default: INFO)

### Dependencies

**Internal Dependencies:**
- `flyingdarts-auth`: Authentication service package from monorepo

**External Dependencies:**
- `lambda_runtime`: AWS Lambda runtime support
- `tokio`: Async runtime
- `serde`: Serialization/deserialization
- `tracing`: Structured logging
- `aws_lambda_events`: AWS Lambda event types
- `http`: HTTP types and utilities

## Development

### Project Structure

```
authorizer/
├── src/
│   ├── lib.rs          # Main library code
│   └── main.rs         # Lambda entry point
├── examples/
│   └── test_auth.rs    # Example usage
├── Cargo.toml          # Dependencies and metadata
└── README.md           # This documentation
```

### Testing

Run the test suite:

```bash
cargo test
```

Run with verbose output:

```bash
cargo test -- --nocapture
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
- **Clippy**: Rust linter for additional checks
- **rustfmt**: Code formatting
- **cargo test**: Unit and integration tests

Run quality checks:

```bash
cargo clippy
cargo fmt --check
```

## Related Projects

- **flyingdarts-auth**: Core authentication service package
- **flyingdarts-backend-api**: Main backend API that uses this authorizer
- **flyingdarts-cdk**: Infrastructure as Code for deployment

## Troubleshooting

### Common Issues

1. **Authorization Token Missing**: Ensure the `Authorization` header is present in requests
2. **Authress Configuration**: Verify Authress credentials and base URL are correctly set
3. **IAM Permissions**: Ensure the Lambda function has appropriate IAM permissions
4. **Cold Start Performance**: Consider using provisioned concurrency for better performance

### Debugging

Enable debug logging:

```bash
export RUST_LOG=debug
```

Check CloudWatch logs for detailed execution information.

### Performance

- **Memory**: Configure appropriate memory allocation (128MB minimum)
- **Timeout**: Set timeout based on expected response times
- **Concurrency**: Monitor concurrent execution limits

## Security Considerations

- **Token Validation**: Always validate tokens before processing requests
- **Error Messages**: Avoid exposing sensitive information in error responses
- **IAM Policies**: Follow principle of least privilege when generating policies
- **Logging**: Ensure sensitive data is not logged in production

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR

## License

Part of the Flying Darts Turbo monorepo. See root LICENSE file for details. 