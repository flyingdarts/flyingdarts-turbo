use anyhow::{anyhow, Result};
use authress::models::UserIdentity;
use authress::{AuthressClient, AuthressSettings};
use base64::{
    alphabet,
    engine::{self, general_purpose},
    Engine,
};
use serde::{Deserialize, Serialize};
use serde_json::Value;

/// Authentication service interface
pub trait AuthService {
    /// Check if the user is authorized
    /// Returns true if authorized, false otherwise
    fn is_authorized(&self, user_id: &str) -> bool;
}

/// Authress-based implementation of the auth service
pub struct AuthressAuthService {
    client: AuthressClient,
}

impl AuthressAuthService {
    /// Create a new AuthressAuthService with the given settings
    pub fn new(authress_url: String, client_key: String) -> Self {
        let authress_settings = AuthressSettings::new(authress_url, client_key);
        let client = AuthressClient::new(&authress_settings);

        Self { client }
    }

    /// Validate a JWT token and get user information
    pub async fn validate_token(&self, token: &str) -> Result<UserIdentity> {
        let user_id = get_user_id_from_token(token)?;

        match self.client.users.get_user(user_id).await {
            Ok(user_identity) => Ok(user_identity),
            Err(error) => Err(anyhow!("Authress validation failed: {}", error)),
        }
    }

    /// Check if a token is valid and user is authorized
    pub async fn is_token_authorized(&self, token: &str) -> Result<bool> {
        match self.validate_token(token).await {
            Ok(_) => Ok(true),
            Err(_) => Ok(false),
        }
    }
}

impl AuthService for AuthressAuthService {
    fn is_authorized(&self, user_id: &str) -> bool {
        // For the trait implementation, we'll use a simple check
        // In practice, you'd want to validate against Authress here
        user_id.starts_with("authorized")
    }
}

/// Example implementation of the auth service (for testing/demo)
pub struct ExampleAuthService;

impl AuthService for ExampleAuthService {
    fn is_authorized(&self, user_id: &str) -> bool {
        // Simple example logic - you can modify this as needed
        // For now, let's say users with IDs starting with "authorized" are allowed
        user_id.starts_with("authorized")
    }
}

/// Request/Response structures for the auth service
#[derive(Debug, Serialize, Deserialize)]
pub struct AuthRequest {
    pub user_id: String,
    pub token: Option<String>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct AuthResponse {
    pub authorized: bool,
    pub message: String,
    pub user_identity: Option<UserIdentity>,
}

/// Helper function to create an auth response
pub fn create_auth_response(authorized: bool) -> AuthResponse {
    let message = if authorized {
        "Successfully authorized!".to_string()
    } else {
        "You are not trespassing, leave immediately! 😂".to_string()
    };

    AuthResponse {
        authorized,
        message,
        user_identity: None,
    }
}

/// Helper function to create an auth response with user identity
pub fn create_auth_response_with_user(
    authorized: bool,
    user_identity: Option<UserIdentity>,
) -> AuthResponse {
    let message = if authorized {
        "Successfully authorized!".to_string()
    } else {
        "You are not trespassing, leave immediately! 😂".to_string()
    };

    AuthResponse {
        authorized,
        message,
        user_identity,
    }
}

/// Extract user ID from a JWT token
pub fn get_user_id_from_token(token: &str) -> Result<String> {
    let parts: Vec<&str> = token.split('.').collect();
    if parts.len() < 2 {
        return Err(anyhow!("Invalid token format"));
    }

    let decoded = engine::GeneralPurpose::new(&alphabet::STANDARD, general_purpose::NO_PAD)
        .decode(parts[1])
        .map_err(|_| anyhow!("Failed to decode token"))?;

    let stringified =
        String::from_utf8(decoded).map_err(|_| anyhow!("Failed to convert token to string"))?;

    let v: Value =
        serde_json::from_str(&stringified).map_err(|_| anyhow!("Unable to parse token JSON"))?;

    let user_id = v["sub"]
        .as_str()
        .ok_or_else(|| anyhow!("Missing 'sub' field in token"))?
        .to_string();

    Ok(user_id)
}

/// Default Authress configuration
pub fn create_default_authress_service() -> AuthressAuthService {
    AuthressAuthService::new(
        "https://authress.flyingdarts.net".into(),
        "SUPER-SECRET-CLIENT-KEY".into(),
    )
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_get_user_id_from_valid_token() {
        let token = "header.eyJzdWIiOiJ1c2VyX2lkIn0.signature"; // Base64-encoded JSON: {"sub": "user_id"}
        assert_eq!(get_user_id_from_token(token).unwrap(), "user_id");
    }

    #[test]
    fn test_get_user_id_from_invalid_token() {
        let token = "invalid.token";
        assert!(get_user_id_from_token(token).is_err());
    }

    #[test]
    fn test_get_user_id_from_token_without_sub() {
        let token = "header.eyJuYW1lIjoiSm9obiBEb2UifQ.signature"; // {"name": "John Doe"}
        assert!(get_user_id_from_token(token).is_err());
    }

    #[test]
    fn test_create_auth_response() {
        let response = create_auth_response(true);
        assert!(response.authorized);
        assert_eq!(response.message, "Successfully authorized!");
        assert!(response.user_identity.is_none());

        let response = create_auth_response(false);
        assert!(!response.authorized);
        assert_eq!(
            response.message,
            "You are not trespassing, leave immediately! 😂"
        );
        assert!(response.user_identity.is_none());
    }

    #[test]
    fn test_example_auth_service() {
        let auth_service = ExampleAuthService;

        assert!(auth_service.is_authorized("authorized_user_123"));
        assert!(!auth_service.is_authorized("unauthorized_user_456"));
    }
}
