namespace Flyingdarts.Infrastructure.Constructs.v2;

public class AuthorizersConstruct : Construct
{
    public WebSocketLambdaAuthorizer WebSocketApiConnectAuthorizer { get; }
    public RequestAuthorizer UsersAuthorizer { get; }
    public RequestAuthorizer StatsAuthorizer { get; }
    public RequestAuthorizer TournamentsAuthorizer { get; }
    
    public AuthorizersConstruct(Construct scope, string id, string environment,LambdaConstruct lambdaConstruct) : base(scope, id)
    {
        UsersAuthorizer = new RequestAuthorizer(this, $"Flyingdarts-Backend-Api-UsersAuthorizer-{environment}-v2",
            new RequestAuthorizerProps
            {
                AuthorizerName = $"FlyingdartsBackendApiUsersAuthorizer{environment}",
                Handler = lambdaConstruct.AuthLambda,
                IdentitySources = new[] { IdentitySource.Header("Authorization") }
            });
        StatsAuthorizer = new RequestAuthorizer(this, $"Flyingdarts-Backend-Api-StatsAuthorizer-{environment}-v2",
            new RequestAuthorizerProps
            {
                AuthorizerName = $"FlyingdartsBackendApiStatsAuthorizer{environment}",
                Handler = lambdaConstruct.AuthLambda,
                IdentitySources = new[] { IdentitySource.Header("Authorization") }
            });
        TournamentsAuthorizer = new RequestAuthorizer(this, $"Flyingdarts-Backend-Api-TournamentsAuthorizer-{environment}-v2",
            new RequestAuthorizerProps
            {
                AuthorizerName = $"FlyingdartsBackendApiTournamentsAuthorizerAuthorizer{environment}",
                Handler = lambdaConstruct.AuthLambda,
                IdentitySources = new[] { IdentitySource.Header("Authorization") }
            });

        WebSocketApiConnectAuthorizer = new WebSocketLambdaAuthorizer(
            $"Flyingdarts-Backend-Api-OnConnect-Authorizer-{environment}", lambdaConstruct.AuthLambda,
            new WebSocketLambdaAuthorizerProps
            {
                AuthorizerName = $"FlyingdartsBackendApiOnConnectAuthorizer{environment}",
                IdentitySource = new[] { "route.request.querystring.token" }
            });
    }
}