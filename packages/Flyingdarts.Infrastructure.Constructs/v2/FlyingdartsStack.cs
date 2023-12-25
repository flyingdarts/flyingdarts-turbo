namespace Flyingdarts.Infrastructure.Constructs.v2;

public class FlyingdartsStack : Stack
{
    private DynamoDbConstruct DynamoDbConstruct { get; }
    private QueueConstruct QueueConstruct { get; }
    private LambdaConstruct LambdaConstruct { get; }
    private ApiGatewayConstruct ApiGatewayConstruct { get; }
    
    public FlyingdartsStack(Construct scope, string environment, IStackProps props) : base(scope, $"FD-Stack-{environment}", props)
    {
        DynamoDbConstruct = new DynamoDbConstruct(this, $"Tables-{environment}", environment);
        QueueConstruct = new QueueConstruct(this, $"Queues-{environment}", environment);
        LambdaConstruct = new LambdaConstruct(this, $"Lambdas-{environment}", environment, DynamoDbConstruct, QueueConstruct);
        ApiGatewayConstruct = new ApiGatewayConstruct(this, $"Apis-{environment}", environment, LambdaConstruct);
        
        new CfnOutput(this, $"WebSocket-Api-Url-Output-{environment}", new CfnOutputProps
        {
            ExportName = $"WebSocketApiUrl{environment}",
            Value = ApiGatewayConstruct.WebSocketStage.Url
        });

        new CfnOutput(this, $"Users-RestApi-Url-Output-{environment}", new CfnOutputProps
        {
            ExportName = $"UsersRestApiUrl{environment}",
            Value = ApiGatewayConstruct.UsersApi.Url
        });

        new CfnOutput(this, $"Tournaments-RestApi-Url-Output-{environment}", new CfnOutputProps
        {
            ExportName = $"TournamentsRestApiUrl{environment}",
            Value = ApiGatewayConstruct.TournamentsApi.Url
        });

        new StringParameter(this, $"SignallingTable-DynamoDb-StringParameter-{environment}", new StringParameterProps
        {
            StringValue = DynamoDbConstruct.SignallingTable.TableName,
            ParameterName = $"/{environment}/Application/DynamoDbTableName"
        });
        
        new StringParameter(this, $"ApplicationTable-DynamoDb-StringParameter-{environment}", new StringParameterProps
        {
            StringValue = DynamoDbConstruct.ApplicationTable.TableName,
            ParameterName = $"/{environment}/Signalling/DynamoDbTableName"
        });

        new StringParameter(this, $"X01StateTable-DynamoDb-StringParameter-{environment}", new StringParameterProps
        {
            StringValue = DynamoDbConstruct.X01StateTable.TableName,
            ParameterName = $"/{environment}/X01State/DynamoDbTableName"
        });
        
        Aspects.Of(this).Add(new Tag("Environment", environment));
        Aspects.Of(this).Add(new Tag("App", "FD-V1"));
    }
}