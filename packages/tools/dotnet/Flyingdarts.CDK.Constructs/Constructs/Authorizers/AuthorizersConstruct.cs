namespace Flyingdarts.CDK.Constructs;

public class AuthorizersConstruct : Construct
{
    public WebSocketLambdaAuthorizer WebSocketApiConnectAuthorizer { get; }
    public RequestAuthorizer RestApiAuthorizer { get; }

    public AuthorizersConstruct(Construct scope, string id, AuthorizersConstructProps props)
        : base(scope, id)
    {
        RestApiAuthorizer = new RequestAuthorizer(
            this,
            props.GetResourceIdentifier(nameof(RestApiAuthorizer)),
            new RequestAuthorizerProps
            {
                AuthorizerName =
                    $"Flyingdarts Rest Api Authorizer {props.DeploymentEnvironment.Name}".Replace(
                        " ",
                        ""
                    ),
                Handler = props.AuthorizerFunction,
                IdentitySources = [IdentitySource.Header("Authorization")],
            }
        );

        WebSocketApiConnectAuthorizer = new WebSocketLambdaAuthorizer(
            props.GetResourceIdentifier(nameof(WebSocketApiConnectAuthorizer)),
            props.AuthorizerFunction,
            new WebSocketLambdaAuthorizerProps
            {
                AuthorizerName =
                    $"Flyingdarts WebSocket Api Authorizer {props.DeploymentEnvironment.Name}".Replace(
                        " ",
                        ""
                    ),
                IdentitySource = ["route.request.querystring.token"],
            }
        );
    }
}
