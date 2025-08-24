namespace Flyingdarts.CDK.Constructs;

public abstract class BaseStackProps
{
    protected abstract string StackName { get; }
    public required DeploymentEnvironment DeploymentEnvironment { get; init; }
    public required Amazon.CDK.Environment StackEnvironment { get; init; }

    public string GetUniqueResourceId(string resourceName)
    {
        return $"{StackName}-{resourceName}-{DeploymentEnvironment.Name}";
    }

    public string StackId
    {
        get
        {
            if (DeploymentEnvironment.Name == DeploymentEnvironment.None.Name)
                return StackName;

            return $"{StackName}-{DeploymentEnvironment.Name}";
        }
    }
}
