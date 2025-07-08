namespace Flyingdarts.Backend.Games.X01.Queue.Models;

public class SQSRecordSet 
{
    public SQSEvent.SQSMessage[] Records { get; set; }
}