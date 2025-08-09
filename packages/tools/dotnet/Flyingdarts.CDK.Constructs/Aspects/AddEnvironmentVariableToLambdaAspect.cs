namespace Flyingdarts.CDK.Constructs;

public class AddEnvironmentVariableToLambdaAspect : DeputyBase, IAspect
{
    private readonly string variableName;
    private readonly string variableValue;

    public AddEnvironmentVariableToLambdaAspect(string variableName, string variableValue)
    {
        this.variableName = variableName;
        this.variableValue = variableValue;
    }

    public void Visit(IConstruct node)
    {
        if (node is Function function)
        {
            function.AddEnvironment(variableName, variableValue);
        }
    }
}
