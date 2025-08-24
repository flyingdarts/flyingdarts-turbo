namespace Flyingdarts.CDK.Constructs;

public class FrontendStackProps : BaseStackProps
{
    public required IHostedZone HostedZone { get; init; }
    public required ICertificate Certificate { get; init; }
    protected override string StackName => Constants.Stacks.Frontend;
}
