namespace Flyingdarts.CDK.Constructs;

/// <summary>
/// This construct is a collection of the resources required for hosting and serving (envShort).flyingdarts.net .
/// When the time comes and we need to have a 'frontend' page aka marketing page, we need to create a new construct for that!
/// Or when we want to setup storybook / widgetbook hosting...
/// </summary>
public class WebApplicationHostingConstructProps : BaseConstructProps
{
    public required string AppName { get; init; }
    public required IHostedZone HostedZone { get; init; }
    public required ICertificate Certificate { get; init; }
    protected override string ConstructName => $"WebApp-{AppName}";
}
