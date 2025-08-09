use flyingdarts_auth::{
    create_auth_response, create_default_authress_service, get_user_id_from_token, AuthService, ExampleAuthService,
};

#[tokio::main]
async fn main() {
    println!("🚀 Flyingdarts Auth Service Demo\n");

    // Test the basic auth service
    println!("=== Basic Auth Service ===");
    let auth_service = ExampleAuthService;

    let test_users = vec![
        "authorized_user_123",
        "authorized_admin_456",
        "unauthorized_user_789",
        "guest_user_101",
        "authorized_test_user",
    ];

    for user_id in test_users {
        println!("Testing user: {}", user_id);

        let is_authorized = auth_service.is_authorized(user_id);
        let auth_response = create_auth_response(is_authorized);

        println!("  Result: {}", auth_response.message);
        println!("  Authorized: {}\n", is_authorized);
    }

    // Test token parsing
    println!("=== Token Parsing ===");
    let test_tokens = vec![
        "header.eyJzdWIiOiJ1c2VyX2lkIn0.signature",
        "header.eyJuYW1lIjoiSm9obiBEb2UifQ.signature",
        "invalid.token",
    ];

    for token in test_tokens {
        println!("Testing token: {}", token);

        match get_user_id_from_token(token) {
            Ok(user_id) => {
                println!("  Successfully extracted user ID: {}", user_id);
            }
            Err(error) => {
                println!("  Failed to extract user ID: {}", error);
            }
        }
        println!();
    }

    // Test Authress service (this would require actual Authress credentials)
    println!("=== Authress Service ===");
    println!("Note: Authress service requires actual credentials to test");
    println!("Creating Authress service with default settings...");

    let authress_service = create_default_authress_service();
    println!("Authress service created successfully!");
    println!("(In a real scenario, this would validate tokens against Authress)");

    println!("\n✨ Demo completed!");
}
