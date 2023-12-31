using Amazon.CDK.AWS.Cognito;

namespace Flyingdarts.Infrastructure.Constructs.v2;

public class AuthorizersConstruct : Construct
{
    private IUserPool CognitoUserPool { get; }
    public CognitoUserPoolsAuthorizer UsersApiAuthorizer { get; }
    public CognitoUserPoolsAuthorizer TournamentsApiAuthorizer { get; }
    
    public WebSocketLambdaAuthorizer WebSocketApiConnectAuthorizer { get; }
    
    public AuthorizersConstruct(Construct scope, string id, string environment,LambdaConstruct lambdaConstruct) : base(scope, id)
    {
        CognitoUserPool = UserPool.FromUserPoolArn(this, $"Flyingdarts-{environment}-Pool",
            $"arn:aws:cognito-idp:{System.Environment.GetEnvironmentVariable("AWS_REGION")!}:{System.Environment.GetEnvironmentVariable("AWS_ACCOUNT")!}:userpool/{System.Environment.GetEnvironmentVariable("AWS_REGION")!}_eJ59ibbay");
        
        UsersApiAuthorizer = new CognitoUserPoolsAuthorizer(this,
            $"Flyingdarts-Backend-Users-RestApi-Cognito-Authorizer-{environment}",
            new CognitoUserPoolsAuthorizerProps
            {
                IdentitySource = IdentitySource.Header("Authorization"),
                AuthorizerName = $"FlyingdartsBackendUsersRestApiCognitoAuthorizer{environment}",
                CognitoUserPools = new []{ CognitoUserPool }
            });
        
        TournamentsApiAuthorizer = new CognitoUserPoolsAuthorizer(this,
            $"Flyingdarts-Backend-Tournaments-RestApi-Cognito-Authorizer-{environment}",
            new CognitoUserPoolsAuthorizerProps
            {
                IdentitySource = IdentitySource.Header("Authorization"),
                AuthorizerName = $"FlyingdartsBackendTournementsRestApiCognitoAuthorizer{environment}",
                CognitoUserPools = new []{ CognitoUserPool }
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