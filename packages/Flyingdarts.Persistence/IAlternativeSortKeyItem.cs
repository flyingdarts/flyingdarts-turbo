namespace Flyingdarts.Persistence;

public interface IAlternativeSortKeyItem
{
    [DynamoDBRangeKey("LSI1")]
    // ReSharper disable once InconsistentNaming
    public string LSI1 { get; set; }
}