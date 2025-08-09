namespace Flyingdarts.CDK.Constructs;

public class LambdaConstructProps : BaseConstructProps
{
    protected override string ConstructName => "Lambda";
    public DynamoDbConstruct DynamoDbConstruct { get; set; }
}
