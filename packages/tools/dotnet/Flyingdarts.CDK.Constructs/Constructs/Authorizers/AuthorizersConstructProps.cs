namespace Flyingdarts.CDK.Constructs;

public class AuthorizersConstructProps : BaseConstructProps
{
    protected override string ConstructName => "AuthorizersConstruct";
    public Function AuthorizerFunction { get; set; }
}
