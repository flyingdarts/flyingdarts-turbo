namespace Flyingdarts.CDK.Constructs;

public abstract class BaseConstructProps
{
    protected abstract string ConstructName { get; }
    public DeploymentEnvironment DeploymentEnvironment { get; set; }

    public virtual string GetResourceIdentifier(string resource)
    {
        return $"{ConstructName}-{resource}-{DeploymentEnvironment.Name}";
    }

    public string ConstructId => $"{ConstructName}-Construct-{DeploymentEnvironment.Name}";
}
