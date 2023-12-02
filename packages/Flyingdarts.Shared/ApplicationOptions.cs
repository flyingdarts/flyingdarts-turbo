namespace Flyingdarts.Shared;
public class ApplicationOptions
{
    public DynamoDBOperationConfig ToOperationConfig()
    {
        return new DynamoDBOperationConfig { OverrideTableName = System.Environment.GetEnvironmentVariable("TableName") };
    }
}
