using System.Reflection.Metadata;

namespace Flyingdarts.CDK.Constructs;

public class DynamoDbConstructProps : BaseConstructProps
{
    public string GetTableName(string tableName) =>
        $"{Constants.PlatformName}-{tableName}-Table-{DeploymentEnvironment.Name}";

    protected override string ConstructName => "DynamoDb";
}
