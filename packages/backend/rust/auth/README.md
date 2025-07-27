# Flying Darts Auth Package

## Overview

The Flying Darts Auth Package is a Rust library that provides authentication and authorization services for the Flying Darts Turbo platform. It offers a flexible authentication system with support for Authress integration, JWT token validation, and customizable authorization logic.

This package is responsible for:
- Providing a unified authentication service interface
- Integrating with Authress for enterprise authentication
- Validating JWT tokens and extracting user information
- Supporting both production and testing authentication scenarios
- Offering helper functions for common authentication tasks
- Providing extensible authorization logic

## Features

- **Flexible Authentication Interface**: Trait-based design for easy customization
- **Authress Integration**: Enterprise-grade authentication via Authress service
- **JWT Token Validation**: Comprehensive JWT token parsing and validation
- **User Identity Management**: Extract and manage user identity information
- **Testing Support**: Example implementation for testing and development
- **Error Handling**: Robust error handling with anyhow integration
- **Async Support**: Full async/await support for token validation
- **Serialization**: JSON serialization for request/response structures

## Prerequisites

- Rust 1.70+ with Cargo
- Authress account and configuration (for production use)
- tokio runtime for async operations

## Installation

Add the package to your `Cargo.toml`:

```toml
[dependencies]
flyingdarts-auth = { path = "../../packages/backend/rust/auth" }
```

Or use it as a workspace dependency:

```toml
[dependencies]
flyingdarts-auth = { workspace = true }
```

## Usage

### Basic Authentication

```rust
use flyingdarts_auth::{AuthService, ExampleAuthService};

let auth_service = ExampleAuthService;

if auth_service.is_authorized("user_123") {
    println!("User is authorized!");
} else {
    println!("User is not authorized!");
}
```

### Authress Integration

```rust
use flyingdarts_auth::{AuthressAuthService, create_default_authress_service};

// Create Authress service with custom configuration
let auth_service = AuthressAuthService::new(
    "https://authress.flyingdarts.net".into(),
    "your-client-key".into(),
);

// Or use default configuration
let auth_service = create_default_authress_service();

// Validate token
let token = "your-jwt-token";
match auth_service.validate_token(token).await {
    Ok(user_identity) => {
        println!("User authorized: {}", user_identity.user_id);
    }
    Err(error) => {
        println!("Authentication failed: {}", error);
    }
}
```

### Token Validation

```rust
use flyingdarts_auth::get_user_id_from_token;

let token = "header.payload.signature";
match get_user_id_from_token(token) {
    Ok(user_id) => {
        println!("User ID: {}", user_id);
    }
    Err(error) => {
        println!("Token validation failed: {}", error);
    }
}
```

### Creating Auth Responses

```rust
use flyingdarts_auth::{create_auth_response, create_auth_response_with_user};

// Simple response
let response = create_auth_response(true);
println!("Message: {}", response.message);

// Response with user identity
let user_identity = Some(user_identity);
let response = create_auth_response_with_user(true, user_identity);
```

## API Reference

### Traits

#### `AuthService`

The main authentication service trait that defines the interface for authorization checks.

**Methods:**

##### `is_authorized(&self, user_id: &str) -> bool`

Checks if a user is authorized based on their user ID.

**Parameters:**
- `user_id` (&str): The user identifier to check

**Returns:**
- `bool`: True if the user is authorized, false otherwise

### Structs

#### `AuthressAuthService`

Production-ready authentication service that integrates with Authress.

**Properties:**
- `client` (AuthressClient): The Authress client instance

**Methods:**

##### `new(authress_url: String, client_key: String) -> Self`

Creates a new AuthressAuthService with the specified configuration.

**Parameters:**
- `authress_url` (String): The Authress service URL
- `client_key` (String): The Authress client key

**Returns:**
- `Self`: New AuthressAuthService instance

##### `validate_token(&self, token: &str) -> Result<UserIdentity>`

Validates a JWT token and returns the user identity.

**Parameters:**
- `token` (&str): The JWT token to validate

**Returns:**
- `Result<UserIdentity>`: User identity on success, error on failure

**Throws:**
- `anyhow::Error`: When token validation fails

##### `is_token_authorized(&self, token: &str) -> Result<bool>`

Checks if a token is valid and the user is authorized.

**Parameters:**
- `token` (&str): The JWT token to check

**Returns:**
- `Result<bool>`: True if authorized, false if not, error on validation failure

#### `ExampleAuthService`

Simple authentication service for testing and development.

**Methods:**

##### `is_authorized(&self, user_id: &str) -> bool`

Implements simple authorization logic for testing.

**Parameters:**
- `user_id` (&str): The user identifier to check

**Returns:**
- `bool`: True if user_id starts with "authorized", false otherwise

#### `AuthRequest`

Request structure for authentication operations.

**Properties:**
- `user_id` (String): User identifier
- `token` (Option<String>): Optional JWT token

#### `AuthResponse`

Response structure for authentication operations.

**Properties:**
- `authorized` (bool): Whether the user is authorized
- `message` (String): Human-readable response message
- `user_identity` (Option<UserIdentity>): Optional user identity information

### Functions

#### `create_auth_response(authorized: bool) -> AuthResponse`

Creates a standard authentication response.

**Parameters:**
- `authorized` (bool): Whether the user is authorized

**Returns:**
- `AuthResponse`: Standard response with appropriate message

#### `create_auth_response_with_user(authorized: bool, user_identity: Option<UserIdentity>) -> AuthResponse`

Creates an authentication response with user identity information.

**Parameters:**
- `authorized` (bool): Whether the user is authorized
- `user_identity` (Option<UserIdentity>): Optional user identity

**Returns:**
- `AuthResponse`: Response with user identity information

#### `get_user_id_from_token(token: &str) -> Result<String>`

Extracts the user ID from a JWT token.

**Parameters:**
- `token` (&str): The JWT token to parse

**Returns:**
- `Result<String>`: User ID on success, error on failure

**Throws:**
- `anyhow::Error`: When token parsing fails

#### `create_default_authress_service() -> AuthressAuthService`

Creates an AuthressAuthService with default configuration.

**Returns:**
- `AuthressAuthService`: Service with default Authress settings

## Configuration

### Environment Variables

- `AUTHRESS_URL`: Authress service URL
- `AUTHRESS_CLIENT_KEY`: Authress client key
- `AUTHRESS_APPLICATION_ID`: Authress application identifier

### Default Configuration

The default Authress configuration uses:
- URL: `https://authress.flyingdarts.net`
- Client Key: `SUPER-SECRET-CLIENT-KEY` (should be overridden in production)

## Dependencies

### External Dependencies

- **serde**: Serialization and deserialization
- **serde_json**: JSON handling with preserve_order feature
- **authress**: Authress SDK for authentication
- **base64**: Base64 encoding/decoding for JWT tokens
- **tokio**: Async runtime support
- **anyhow**: Error handling

### Internal Dependencies

None - this is a standalone authentication library.

## Development

### Project Structure

```
auth/
├── src/
│   └── lib.rs              # Main library code
├── tests/                  # Integration tests
├── Cargo.toml             # Dependencies and metadata
├── package.json           # Node.js package metadata
└── README.md              # This documentation
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

## Integration Examples

### Lambda Function Integration

```rust
use flyingdarts_auth::{AuthressAuthService, create_auth_response};

pub async fn handler(event: LambdaEvent<Request>) -> Result<Response, Error> {
    let auth_service = AuthressAuthService::new(
        std::env::var("AUTHRESS_URL")?,
        std::env::var("AUTHRESS_CLIENT_KEY")?,
    );

    let token = event.payload.authorization_token;
    let is_authorized = auth_service.is_token_authorized(&token).await?;

    Ok(create_auth_response(is_authorized))
}
```

### Web Service Integration

```rust
use flyingdarts_auth::{get_user_id_from_token, AuthressAuthService};

pub async fn authenticate_request(token: &str) -> Result<String, Error> {
    let user_id = get_user_id_from_token(token)?;
    
    let auth_service = create_default_authress_service();
    let user_identity = auth_service.validate_token(token).await?;
    
    Ok(user_identity.user_id)
}
```

## Security Considerations

- **Token Validation**: Always validate JWT tokens before processing
- **Error Messages**: Avoid exposing sensitive information in error responses
- **Configuration**: Use environment variables for sensitive configuration
- **Token Storage**: Never store tokens in logs or error messages
- **HTTPS**: Always use HTTPS for token transmission

### Best Practices

1. **Use Strong Tokens**: Ensure Authress generates strong, time-limited tokens
2. **Validate All Requests**: Never skip token validation for any endpoint
3. **Monitor Access**: Log authentication attempts for security monitoring
4. **Regular Updates**: Keep Authress SDK and dependencies updated
5. **Security Audits**: Regularly audit authentication patterns

## Related Projects

- **flyingdarts-authorizer**: Rust Lambda authorizer that uses this package
- **flyingdarts-auth-api**: .NET authentication API
- **flyingdarts-mobile**: Flutter app with authentication
- **flyingdarts-web**: Angular app with authentication

## Troubleshooting

### Common Issues

1. **Token Validation Failures**: Check token format and Authress configuration
2. **Authress Connection Issues**: Verify Authress service URL and credentials
3. **JWT Parsing Errors**: Ensure tokens are properly formatted
4. **Async Runtime Issues**: Ensure tokio runtime is available

### Debugging

Enable debug logging:

```rust
// In your code
println!("Debug: Token validation result: {:?}", result);
```

Check Authress logs for detailed authentication information.

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure all tests pass before submitting PR
5. Follow security best practices for authentication code

## License

Part of the Flying Darts Turbo monorepo. See root LICENSE file for details. 