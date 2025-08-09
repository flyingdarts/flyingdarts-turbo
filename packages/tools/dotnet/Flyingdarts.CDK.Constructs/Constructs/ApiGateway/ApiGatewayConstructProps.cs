namespace Flyingdarts.CDK.Constructs;

public class ApiGatewayConstructProps : BaseConstructProps
{
    public LambdaConstruct LambdaConstruct { get; set; }
    public AuthorizersConstruct AuthorizersConstruct { get; set; }
    protected override string ConstructName => "ApiGateway";
}
