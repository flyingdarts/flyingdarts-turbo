using System.Text;

namespace Flyingdarts.Persistence;

public class ApplicationOptions
{
    public DynamoDBOperationConfig ToOperationConfig()
    {
        return new DynamoDBOperationConfig { OverrideTableName = Environment.GetEnvironmentVariable("TableName") };
    }

    public string GetDyteAuthorizationHeaderValue()
    {
        var dyteOrganizationId = Environment.GetEnvironmentVariable("DyteOrganizationId");
        var dyteApiKey = Environment.GetEnvironmentVariable("DyteApiKey");
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{dyteOrganizationId}:{dyteApiKey}"));
    }
}
