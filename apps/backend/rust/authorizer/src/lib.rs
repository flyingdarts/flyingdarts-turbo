use aws_lambda_events::apigw::{ApiGatewayProxyRequest, ApiGatewayProxyResponse};
use http::HeaderMap;
use lambda_runtime::{Error, LambdaEvent};
use serde::{Deserialize, Serialize};
use tracing::info;

use flyingdarts_auth::{
    create_auth_response, create_auth_response_with_user, create_default_authress_service,
    AuthService, ExampleAuthService,
};

#[derive(Deserialize)]
pub struct Request {
    #[serde(rename = "type")]
    pub _type: String,
    pub method_arn: String,
    pub authorization_token: String,
}

#[derive(Serialize)]
pub struct Response {
    pub principal_id: String,
    pub policy_document: PolicyDocument,
    pub context: serde_json::Value,
}

#[derive(Serialize)]
pub struct PolicyDocument {
    pub version: String,
    pub statement: Vec<Statement>,
}

#[derive(Serialize)]
pub struct Statement {
    pub action: String,
    pub effect: String,
    pub resource: String,
}

pub async fn function_handler(event: LambdaEvent<Request>) -> Result<Response, Error> {
    let request = event.payload;

    info!(
        "Processing authorization request for token: {}",
        request.authorization_token
    );

    // Create an instance of the auth service
    let auth_service = ExampleAuthService;

    // Use the authorization token as the user_id for this example
    // In a real scenario, you would decode the token to get the actual user_id
    let user_id = &request.authorization_token;

    // Check if the user is authorized using our auth service
    let is_authorized = auth_service.is_authorized(user_id);

    // Create the auth response with the appropriate message
    let auth_response = create_auth_response(is_authorized);

    info!("Auth result: {}", auth_response.message);

    // Create the IAM policy response
    let effect = if is_authorized { "Allow" } else { "Deny" };

    let response = Response {
        principal_id: user_id.to_string(),
        policy_document: PolicyDocument {
            version: "2012-10-17".to_string(),
            statement: vec![Statement {
                action: "execute-api:Invoke".to_string(),
                effect: effect.to_string(),
                resource: request.method_arn,
            }],
        },
        context: serde_json::json!({
            "message": auth_response.message,
            "authorized": auth_response.authorized
        }),
    };

    Ok(response)
}

/// Handler for API Gateway proxy requests with Authress integration
pub async fn authress_handler(
    event: LambdaEvent<ApiGatewayProxyRequest>,
) -> Result<ApiGatewayProxyResponse, Error> {
    let request = event.payload;

    info!("Processing API Gateway request with Authress validation");

    // Extract authorization header
    let auth_header = match request.headers.get("Authorization") {
        Some(header) => header.to_str().map_err(|_| {
            Error::from(std::io::Error::new(
                std::io::ErrorKind::InvalidData,
                "Invalid Authorization header",
            ))
        })?,
        None => {
            return Ok(create_api_response(
                401,
                Some("Missing Authorization header".into()),
            ));
        }
    };

    // Create Authress service
    let authress_service = create_default_authress_service();

    // Validate token with Authress
    match authress_service.validate_token(auth_header).await {
        Ok(user_identity) => {
            info!("User authorized: {}", user_identity.user_id);

            let auth_response = create_auth_response_with_user(true, Some(user_identity.clone()));

            Ok(create_api_response(
                200,
                Some(format!(
                    "{} - User: {}",
                    auth_response.message,
                    user_identity.email.unwrap_or(user_identity.user_id)
                )),
            ))
        }
        Err(error) => {
            info!("Authorization failed: {}", error);

            let auth_response = create_auth_response(false);

            Ok(create_api_response(401, Some(auth_response.message)))
        }
    }
}

/// Create an API Gateway proxy response
fn create_api_response(status_code: i64, body_content: Option<String>) -> ApiGatewayProxyResponse {
    let empty_headers = HeaderMap::new();
    let body = body_content.map(aws_lambda_events::encodings::Body::Text);

    ApiGatewayProxyResponse {
        status_code,
        multi_value_headers: empty_headers.clone(),
        headers: empty_headers,
        body,
        is_base64_encoded: false,
    }
}

#[cfg(test)]
mod tests {
    use flyingdarts_auth::{create_auth_response, AuthService, ExampleAuthService};

    #[test]
    fn test_auth_service_integration() {
        let auth_service = ExampleAuthService;

        // Test authorized user
        let authorized_user = "authorized_user_123";
        let is_authorized = auth_service.is_authorized(authorized_user);
        let auth_response = create_auth_response(is_authorized);

        assert!(is_authorized);
        assert_eq!(auth_response.message, "Successfully authorized!");

        // Test unauthorized user
        let unauthorized_user = "unauthorized_user_456";
        let is_authorized = auth_service.is_authorized(unauthorized_user);
        let auth_response = create_auth_response(is_authorized);

        assert!(!is_authorized);
        assert_eq!(
            auth_response.message,
            "You are not trespassing, leave immediately! 😂"
        );
    }
}
