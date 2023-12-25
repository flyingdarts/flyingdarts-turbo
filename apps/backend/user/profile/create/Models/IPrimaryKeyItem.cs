using Amazon.DynamoDBv2.DataModel;

public interface IPrimaryKeyItem
{
    [DynamoDBRangeKey("PK")]
    public string PrimaryKey { get; set; }
}