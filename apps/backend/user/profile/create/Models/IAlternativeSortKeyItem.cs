using Amazon.DynamoDBv2.DataModel;

public interface IAlternativeSortKeyItem
{
    [DynamoDBRangeKey("LSI1")]
    // ReSharper disable once InconsistentNaming
    public string LSI1 { get; set; }
}