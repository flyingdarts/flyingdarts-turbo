namespace Flyingdarts.CDK.Constructs;

public class GithubActionsOIDCConstructProps : BaseConstructProps
{
    public required string Repository { get; init; }
    protected override string ConstructName => "GithubActions";
}
