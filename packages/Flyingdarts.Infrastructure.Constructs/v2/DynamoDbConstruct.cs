class DynamoDbConstruct : Construct
{
    private Table X01StateTable;
    public DynamoDbConstruct(Construct scope, string id) : base(scope, id)
    {
        X01StateTable = new Table(this, "Flyingdarts-X01State-Table", new TableProps
        {
            TableName = "Flyingdarts-X01State-Table",
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            BillingMode = BillingMode.PAY_PER_REQUEST,
            Encryption = TableEncryption.AWS_MANAGED,
            PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
            SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });
    }
}