namespace Flyingdarts.CDK.Constructs;

public class ApiGatewayConstructProps : BaseConstructProps
{
    public required LambdaConstruct LambdaConstruct { get; init; }
    public required AuthorizersConstruct AuthorizersConstruct { get; init; }
    protected override string ConstructName => "ApiGateway";
}
