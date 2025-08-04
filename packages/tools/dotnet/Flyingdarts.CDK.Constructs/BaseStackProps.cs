namespace Flyingdarts.CDK.Constructs;

public abstract class BaseStackProps
{
    protected abstract string StackName { get; }
    public DeploymentEnvironment DeploymentEnvironment { get; set; }
    public Amazon.CDK.Environment StackEnvironment { get; set; }

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
