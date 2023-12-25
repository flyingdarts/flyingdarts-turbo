using Amazon.DynamoDBv2.DataModel;

public interface ISortKeyItem
{
    [DynamoDBRangeKey("SK")]
    public string SortKey { get; set; }
}