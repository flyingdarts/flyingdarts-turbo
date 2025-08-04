namespace Flyingdarts.CDK.Constructs;

public class DynamoDbConstruct : Construct
{
    public Table Application { get; }
    public Table X01State { get; }

    public DynamoDbConstruct(Construct scope, DynamoDbConstructProps props)
        : base(scope, props.ConstructId)
    {
        // Single table design. See Flyingdarts.Persistence
        // TODO: Write down access patterns
        Application = CreateTable(
            this,
            props.GetResourceIdentifier(nameof(Application)),
            props.GetTableName(nameof(Application))
        );

        Application.AddLocalSecondaryIndex(
            new LocalSecondaryIndexProps
            {
                IndexName = "LSI1",
                SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
            }
        );

        X01State = CreateTable(
            this,
            props.GetResourceIdentifier(nameof(X01State)),
            props.GetTableName(nameof(X01State)) // Make a enum or struct perhaps to store the table name and easy access
        );
    }

    private static Table CreateTable(DynamoDbConstruct construct, string id, string tableName)
    {
        return new Table(
            construct,
            id,
            new TableProps
            {
                TableName = tableName,
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
