namespace Flyingdarts.Persistence;

public interface IPrimaryKeyItem
{
    [DynamoDBRangeKey("PK")]
    public string PrimaryKey { get; set; }
}