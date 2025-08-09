namespace Flyingdarts.CDK.Constructs;

public class FrontendStackProps : BaseStackProps
{
    public IHostedZone HostedZone { get; set; }
    public ICertificate Certificate { get; set; }
    protected override string StackName => Constants.Stacks.Frontend;
}
