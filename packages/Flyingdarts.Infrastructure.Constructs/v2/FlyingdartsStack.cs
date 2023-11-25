namespace Flyingdarts.Infrastructure.Constructs.v2;

public class FlyingdartsStack : Stack
{
    private DynamoDbConstruct DynamoDbConstruct { get; }
    private QueueConstruct QueueConstruct { get; }
    private LambdaConstruct LambdaConstruct { get; }
    private ApiGatewayConstruct ApiGatewayConstruct { get; }
    
    public FlyingdartsStack(Construct scope, string environment, IStackProps props) : base(scope, $"FD-Stack-{environment}", props)
    {
        DynamoDbConstruct = new DynamoDbConstruct(scope, $"Tables-{environment}", environment);
        QueueConstruct = new QueueConstruct(scope, $"Queues-{environment}", environment);
        LambdaConstruct = new LambdaConstruct(scope, $"Lambdas-{environment}", environment, DynamoDbConstruct, QueueConstruct);
        ApiGatewayConstruct = new ApiGatewayConstruct(scope, $"Apis-{environment}", environment, LambdaConstruct);
        
        new CfnOutput(this, $"WebSocket-Api-Url-Output-{environment}", new CfnOutputProps
        {
            ExportName = "WebSocketApiUrl",
            Value = ApiGatewayConstruct.WebSocketStage.Url
        });
        
        new StringParameter(this, $"SignallingTable-DynamoDb-StringParameter-{environment}", new StringParameterProps
        {
            StringValue = DynamoDbConstruct.SignallingTable.TableName,
            ParameterName = $"/{environment}/Application/DynamoDb"
        });
        
        new StringParameter(this, $"ApplicationTable-DynamoDb-StringParameter-{environment}", new StringParameterProps
        {
            StringValue = DynamoDbConstruct.ApplicationTable.TableName,
            ParameterName = $"/{environment}/Signalling/DynamoDb"
        });

        new StringParameter(this, $"X01StateTable-DynamoDb-StringParameter-{environment}", new StringParameterProps
        {
            StringValue = DynamoDbConstruct.X01StateTable.TableName,
            ParameterName = $"/{environment}/X01State/DynamoDb"
        });
    }
}