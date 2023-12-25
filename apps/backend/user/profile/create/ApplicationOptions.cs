using Amazon.DynamoDBv2.DataModel;
public class ApplicationOptions
{
    public DynamoDBOperationConfig ToOperationConfig()
    {
        return new DynamoDBOperationConfig { OverrideTableName = System.Environment.GetEnvironmentVariable("TableName") };
    }
}
