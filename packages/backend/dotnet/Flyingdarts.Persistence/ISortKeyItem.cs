namespace Flyingdarts.Persistence;

public interface ISortKeyItem
{
    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; }
}