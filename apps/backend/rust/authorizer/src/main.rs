use flyingdarts_authorizer::authress_handler;
use lambda_runtime::{service_fn, Error as LambdaError};
#[tokio::main]
async fn main() -> Result<(), LambdaError> {
    tracing_subscriber::fmt()
        .with_max_level(tracing::Level::INFO)
        .with_target(false)
        .without_time()
        .init();

    lambda_runtime::run(service_fn(authress_handler)).await
}
