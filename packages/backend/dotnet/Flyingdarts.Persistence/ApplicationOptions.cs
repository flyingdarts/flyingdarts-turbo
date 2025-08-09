using System.Text;

namespace Flyingdarts.Persistence;

public class ApplicationOptions
{
    public DynamoDBOperationConfig ToOperationConfig()
    {
        return new DynamoDBOperationConfig
        {
            OverrideTableName = Environment.GetEnvironmentVariable("TableName"),
        };
    }

    public string GetDyteAuthorizationHeaderValue()
    {
        return Environment.GetEnvironmentVariable("DyteAuthorizationHeaderValue")!;
    }
}
