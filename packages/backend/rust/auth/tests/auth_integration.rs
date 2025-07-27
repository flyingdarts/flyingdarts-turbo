use authress::models::UserIdentity;
use flyingdarts_auth::{
    create_auth_response, create_auth_response_with_user, get_user_id_from_token, AuthService,
    ExampleAuthService,
};

#[test]
fn test_example_auth_service_integration() {
    let service = ExampleAuthService;
    assert!(service.is_authorized("authorized_user_123"));
    assert!(!service.is_authorized("unauthorized_user_456"));
}

#[test]
fn test_token_parsing_integration() {
    let valid_token = "header.eyJzdWIiOiJ1c2VyX2lkIn0.signature"; // {"sub": "user_id"}
    let invalid_token = "invalid.token";
    let no_sub_token = "header.eyJuYW1lIjoiSm9obiBEb2UifQ.signature"; // {"name": "John Doe"}

    assert_eq!(get_user_id_from_token(valid_token).unwrap(), "user_id");
    assert!(get_user_id_from_token(invalid_token).is_err());
    assert!(get_user_id_from_token(no_sub_token).is_err());
}

#[test]
fn test_create_auth_response_integration() {
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
fn test_create_auth_response_with_user_integration() {
    let user = UserIdentity {
        user_id: "user_id".to_string(),
        email: Some("user@example.com".to_string()),
        name: Some("Test User".to_string()),
        picture: None,
    };
    let response = create_auth_response_with_user(true, Some(user.clone()));
    assert!(response.authorized);
    assert_eq!(response.message, "Successfully authorized!");
    assert_eq!(response.user_identity.unwrap().user_id, user.user_id);
}

// Note: Integration tests for AuthressAuthService would require live Authress credentials and network access.
// You can add async tests with #[tokio::test] and mock the AuthressClient if needed.
