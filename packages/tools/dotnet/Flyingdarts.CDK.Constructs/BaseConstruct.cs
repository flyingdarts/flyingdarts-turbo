namespace Flyingdarts.CDK.Constructs;

public class BaseConstruct<T> : Construct
    where T : BaseConstructProps
{
    private readonly T ConstructProps;

    public BaseConstruct(Construct scope, T props)
        : base(scope, props.ConstructId)
    {
        ConstructProps = props;
    }

    private string GetPlatformName(string name) => $"{Constants.PlatformName}-{name}";

    protected string GetUniqueName(string name) =>
        $"{GetPlatformName(name)}-{ConstructProps.DeploymentEnvironment.Name}";

    protected string GetUniqueApiName(string name) =>
        $"{GetPlatformName(name)}-Api-{ConstructProps.DeploymentEnvironment.Name}";
}
