using Flyingdarts.CDK.Constructs.v2.Aspects;

namespace Flyingdarts.CDK.Constructs.v2;

public class AuthStack : Stack
{
    private GithubAuthConstruct GithubAuthConstruct { get; }

    public AuthStack(Construct scope, IStackProps props)
        : base(scope, $"Auth-Stack", props)
    {
        GithubAuthConstruct = new GithubAuthConstruct(
            this,
            $"Github-OIDC-Provider",
            new string[] { "flyingdarts-turbo" }
        );
    }
}

public class FlyingdartsStack : Stack
{
    private DynamoDbConstruct DynamoDbConstruct { get; }
    private LambdaConstruct LambdaConstruct { get; }
    private ApiGatewayConstruct ApiGatewayConstruct { get; }
    private AuthorizersConstruct AuthorizersConstruct { get; }

    public FlyingdartsStack(Construct scope, string environment, IStackProps props)
        : base(scope, $"FD-Stack-{environment}", props)
    {
        DynamoDbConstruct = new DynamoDbConstruct(this, $"Tables-{environment}", environment);
        LambdaConstruct = new LambdaConstruct(
            this,
            $"Lambdas-{environment}",
            environment,
            DynamoDbConstruct
        );

        AuthorizersConstruct = new AuthorizersConstruct(
            this,
            $"Authorizers-{environment}",
            environment,
            LambdaConstruct
        );

        ApiGatewayConstruct = new ApiGatewayConstruct(
            this,
            $"Apis-{environment}",
            environment,
            LambdaConstruct,
            AuthorizersConstruct
        );

        new CfnOutput(
            this,
            $"WebSocket-Api-Url-Output-{environment}",
            new CfnOutputProps
            {
                ExportName = $"WebSocketApiUrl{environment}",
                Value = ApiGatewayConstruct.WebSocketStage.Url
            }
        );

        new CfnOutput(
            this,
            $"Friends-RestApi-Url-Output-{environment}",
            new CfnOutputProps
            {
                ExportName = $"FriendsRestApiUrl{environment}",
                Value = ApiGatewayConstruct.FriendsApi.Url
            }
        );
        new StringParameter(
            this,
            $"ApplicationTable-DynamoDb-StringParameter-{environment}",
            new StringParameterProps
            {
                StringValue = DynamoDbConstruct.ApplicationTable.TableName,
                ParameterName = $"/{environment}/Signalling/DynamoDbTableName"
            }
        );

        new StringParameter(
            this,
            $"X01StateTable-DynamoDb-StringParameter-{environment}",
            new StringParameterProps
            {
                StringValue = DynamoDbConstruct.X01StateTable.TableName,
                ParameterName = $"/{environment}/X01State/DynamoDbTableName"
            }
        );

        AmazonAspect.Of(this).Add(new Tag("Environment", environment));
        AmazonAspect.Of(this).Add(new Tag("App", "FD-V1"));
        AmazonAspect
            .Of(this)
            .Add(new AddEnvironmentVariableToLambdaAspect("EnvironmentName", environment));
        AmazonAspect
            .Of(this)
            .Add(new AddEnvironmentVariableToLambdaAspect("LAMBDA_NET_SERIALIZER_DEBUG", "true"));
    }
}
