namespace Flyingdarts.CDK.Constructs;

/// <summary>
/// This construct is a collection of the resources required for hosting and serving (envShort).flyingdarts.net .
/// When the time comes and we need to have a 'frontend' page aka marketing page, we need to create a new construct for that!
/// Or when we want to setup storybook / widgetbook hosting...
/// </summary>
public class WebApplicationHostingConstructProps : BaseConstructProps
{
    public string AppName { get; set; }
    public IHostedZone HostedZone { get; set; }
    public ICertificate Certificate { get; set; }
    protected override string ConstructName => $"WebApp-{AppName}";

    public override string GetResourceIdentifier(string resource) =>
        $"{Constants.Stacks.Frontend}-{AppName}-{resource}-{DeploymentEnvironment.Name}";
}
