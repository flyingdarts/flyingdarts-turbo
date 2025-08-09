namespace Flyingdarts.CDK.Constructs;

public class GithubActionsOIDCConstructProps : BaseConstructProps
{
    public string Repository { get; set; }
    protected override string ConstructName => "GithubActions";
}
