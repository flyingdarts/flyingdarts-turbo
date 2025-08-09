namespace Flyingdarts.CDK.Constructs;

public class BaseStack<T> : Stack
    where T : BaseStackProps
{
    private readonly T StackProps;

    public BaseStack(Construct scope, T props)
        : base(scope, props.StackId, new StackProps { Env = props.StackEnvironment })
    {
        StackProps = props;
    }

    protected string GetCfnOutputExportName(string name) =>
        $"{Constants.PlatformName}{name}{StackProps.DeploymentEnvironment.Name}";
}
