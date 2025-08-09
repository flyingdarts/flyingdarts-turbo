namespace Flyingdarts.CDK.Constructs;

public class BackendStack : BaseStack<BackendStackProps>
{
    private DynamoDbConstruct DynamoDbConstruct { get; }
    private LambdaConstruct LambdaConstruct { get; }
    private ApiGatewayConstruct ApiGatewayConstruct { get; }
    private AuthorizersConstruct AuthorizersConstruct { get; }
    private string WebSocketApiUrl => ApiGatewayConstruct.WebSocketApiUrl;
    private string FriendsApiUrl => ApiGatewayConstruct.FriendsApiUrl;
    private string ApplicationTableName => DynamoDbConstruct.Application.TableName;
    private string X01StateTableName => DynamoDbConstruct.X01State.TableName;

    public BackendStack(Construct scope, BackendStackProps props)
        : base(scope, props)
    {
        DynamoDbConstruct = new DynamoDbConstruct(
            this,
            new DynamoDbConstructProps { DeploymentEnvironment = props.DeploymentEnvironment }
        );
        LambdaConstruct = new LambdaConstruct(
            this,
            new LambdaConstructProps
            {
                DeploymentEnvironment = props.DeploymentEnvironment,
                DynamoDbConstruct = DynamoDbConstruct,
            }
        );
        AuthorizersConstruct = new AuthorizersConstruct(
            this,
            props.GetUniqueResourceId(nameof(AuthorizersConstruct)),
            new AuthorizersConstructProps
            {
                DeploymentEnvironment = props.DeploymentEnvironment,
                AuthorizerFunction = LambdaConstruct.Authorizer,
            }
        );
        ApiGatewayConstruct = new ApiGatewayConstruct(
            this,
            props.GetUniqueResourceId(nameof(ApiGatewayConstruct)),
            new ApiGatewayConstructProps
            {
                DeploymentEnvironment = props.DeploymentEnvironment,
                LambdaConstruct = LambdaConstruct,
                AuthorizersConstruct = AuthorizersConstruct,
            }
        );

        new CfnOutput(
            this,
            props.GetUniqueResourceId($"{nameof(WebSocketApiUrl)}-{nameof(CfnOutput)}"),
            new CfnOutputProps
            {
                ExportName = GetCfnOutputExportName(nameof(WebSocketApiUrl)),
                Value = WebSocketApiUrl,
            }
        );

        new CfnOutput(
            this,
            props.GetUniqueResourceId($"{nameof(FriendsApiUrl)}-{nameof(CfnOutput)}"),
            new CfnOutputProps
            {
                ExportName = GetCfnOutputExportName(nameof(FriendsApiUrl)),
                Value = FriendsApiUrl,
            }
        );

        new StringParameter(
            this,
            props.GetUniqueResourceId($"{nameof(ApplicationTableName)}-{nameof(StringParameter)}"),
            new StringParameterProps
            {
                StringValue = ApplicationTableName,
                ParameterName =
                    $"/{props.DeploymentEnvironment.Name}/Application/DynamoDbTableName",
            }
        );

        new StringParameter(
            this,
            props.GetUniqueResourceId($"{nameof(X01StateTableName)}-{nameof(StringParameter)}"),
            new StringParameterProps
            {
                StringValue = X01StateTableName,
                ParameterName = $"/{props.DeploymentEnvironment.Name}/X01State/DynamoDbTableName",
            }
        );

        AmazonAspect
            .Of(this)
            .Add(new Amazon.CDK.Tag("Environment", props.DeploymentEnvironment.Name));
        AmazonAspect.Of(this).Add(new Amazon.CDK.Tag("App", "Flyingdarts"));
        AmazonAspect
            .Of(this)
            .Add(
                new AddEnvironmentVariableToLambdaAspect(
                    "Environment",
                    props.DeploymentEnvironment.Name
                )
            );
        AmazonAspect
            .Of(this)
            .Add(
                new AddEnvironmentVariableToLambdaAspect(
                    "EnvironmentName",
                    props.DeploymentEnvironment.Name
                )
            );
        AmazonAspect
            .Of(this)
            .Add(new AddEnvironmentVariableToLambdaAspect("LAMBDA_NET_SERIALIZER_DEBUG", "true"));
    }
}
