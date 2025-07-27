namespace Flyingdarts.CDK.Constructs.v2;

public class DynamoDbConstruct : Construct
{
    public Table ApplicationTable { get; }
    public Table X01StateTable { get; }

    public DynamoDbConstruct(Construct scope, string id, string environment)
        : base(scope, id)
    {
        ApplicationTable = new Table(
            this,
            $"Flyingdarts-Application-Table-{environment}",
            new TableProps
            {
                TableName = $"Flyingdarts-Application-Table-{environment}",
                Stream = StreamViewType.NEW_AND_OLD_IMAGES,
                BillingMode = BillingMode.PAY_PER_REQUEST,
                Encryption = TableEncryption.AWS_MANAGED,
                PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
                SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
                RemovalPolicy = RemovalPolicy.DESTROY,
            }
        );

        ApplicationTable.AddLocalSecondaryIndex(
            new LocalSecondaryIndexProps
            {
                IndexName = "LSI1",
                SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
            }
        );

        X01StateTable = new Table(
            this,
            $"Flyingdarts-X01State-Table-{environment}",
            new TableProps
            {
                TableName = $"Flyingdarts-X01State-Table-{environment}",
                Stream = StreamViewType.NEW_AND_OLD_IMAGES,
                BillingMode = BillingMode.PAY_PER_REQUEST,
                Encryption = TableEncryption.AWS_MANAGED,
                PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
                SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
                RemovalPolicy = RemovalPolicy.DESTROY,
            }
        );
    }
}
