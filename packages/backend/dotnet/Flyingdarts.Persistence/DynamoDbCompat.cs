namespace Flyingdarts.Persistence;

public static class DynamoDbCompat
{
    public static IAsyncSearch<T> FromQueryAsyncCompat<T>(
        this IDynamoDBContext context,
        QueryOperationConfig query,
        DynamoDBOperationConfig config
    )
    {
#pragma warning disable CS0618
        return context.FromQueryAsync<T>(query, config);
#pragma warning restore CS0618
    }

    public static IAsyncSearch<T> FromScanAsyncCompat<T>(
        this IDynamoDBContext context,
        ScanOperationConfig scan,
        DynamoDBOperationConfig config
    )
    {
#pragma warning disable CS0618
        return context.FromScanAsync<T>(scan, config);
#pragma warning restore CS0618
    }

    public static IBatchWrite<T> CreateBatchWriteCompat<T>(
        this IDynamoDBContext context,
        DynamoDBOperationConfig config
    )
    {
#pragma warning disable CS0618
        return context.CreateBatchWrite<T>(config);
#pragma warning restore CS0618
    }
}
