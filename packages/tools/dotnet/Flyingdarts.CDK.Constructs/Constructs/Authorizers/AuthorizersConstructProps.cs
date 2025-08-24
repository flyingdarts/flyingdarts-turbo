namespace Flyingdarts.CDK.Constructs;

public class AuthorizersConstructProps : BaseConstructProps
{
    protected override string ConstructName => "AuthorizersConstruct";
    public required Function AuthorizerFunction { get; init; }
}
