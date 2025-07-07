
namespace Flyingdarts.Infrastructure.Constructs.v2;

public class DynamoDbConstruct : Construct
{
    public Table SignallingTable { get; }
    public Table ApplicationTable { get; }
    public Table X01StateTable { get; }
    public Table X01QueueTable { get; }
    public DynamoDbConstruct(Construct scope, string id, string environment) : base(scope, id)
    {
        SignallingTable = new Table(this, $"Flyingdarts-Signalling-Table-{environment}", new TableProps
        {
            TableName = $"Flyingdarts-Signalling-Table-{environment}",
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            BillingMode = BillingMode.PAY_PER_REQUEST,
            Encryption = TableEncryption.AWS_MANAGED,
            PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "ConnectionId" },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        ApplicationTable = new Table(this, $"Flyingdarts-Application-Table-{environment}", new TableProps
        {
            TableName = $"Flyingdarts-Application-Table-{environment}",
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            BillingMode = BillingMode.PAY_PER_REQUEST,
            Encryption = TableEncryption.AWS_MANAGED,
            PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
            SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        ApplicationTable.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "LSI1",
            SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
        });

        X01StateTable = new Table(this, $"Flyingdarts-X01State-Table-{environment}", new TableProps
        {
            TableName = $"Flyingdarts-X01State-Table-{environment}",
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            BillingMode = BillingMode.PAY_PER_REQUEST,
            Encryption = TableEncryption.AWS_MANAGED,
            PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
            SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        X01QueueTable = new Table(this, $"Flyingdarts-X01Queue-Table-{environment}", new TableProps
        {
            TableName = $"Flyingdarts-X01Queue-Table-{environment}",
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            BillingMode = BillingMode.PAY_PER_REQUEST,
            Encryption = TableEncryption.AWS_MANAGED,
            PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
            SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });
    }

}