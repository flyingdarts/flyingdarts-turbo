using Amazon.DynamoDBv2.Model;

namespace Flyingdarts.Persistence;

public class QueueService<T> : IQueueService<T>
    where T : ISortKeyItem
{
    private IDynamoDBContext DbContext;
    private IAmazonDynamoDB DynamoDBClient;

    public QueueService(IDynamoDBContext dbContext, IAmazonDynamoDB dynamoDbClient)
    {
        DbContext = dbContext;
        DynamoDBClient = dynamoDbClient;
    }

    public async Task DeleteRecords(IEnumerable<T> records, CancellationToken cancellationToken)
    {
        foreach (var record in records)
        {
            var request = new DeleteItemRequest
            {
                TableName =
                    $"Flyingdarts-{typeof(T)}-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}",
                Key = new Dictionary<string, AttributeValue>
                {
                    {
                        "PK",
                        new AttributeValue { S = "X01QueueState" }
                    },
                    {
                        "SK",
                        new AttributeValue { S = record.SortKey }
                    }
                }
            };

            await DynamoDBClient.DeleteItemAsync(request);
        }
    }

    public async Task AddRecord(T record, CancellationToken cancellationToken)
    {
        var stateWrite = DbContext.CreateBatchWrite<T>(OperationConfig);

        stateWrite.AddPutItem(record);

        await stateWrite.ExecuteAsync(cancellationToken);
    }

    public async Task<List<T>> GetRecords(CancellationToken cancellationToken)
    {
        var results = await DbContext
            .FromQueryAsync<T>(Query(), OperationConfig)
            .GetRemainingAsync(cancellationToken);
        return results;
    }

    private static QueryOperationConfig Query()
    {
        var queryFilter = new QueryFilter("PK", QueryOperator.Equal, "X01QueueState");
        return new QueryOperationConfig { Filter = queryFilter };
    }

    private DynamoDBOperationConfig OperationConfig
    {
        get
        {
            var stateType = typeof(T);
            var tableName =
                $"Flyingdarts-{stateType}-Table-{Environment.GetEnvironmentVariable("EnvironmentName")}";
            return new DynamoDBOperationConfig { OverrideTableName = tableName };
        }
    }
}
