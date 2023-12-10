using Amazon.Lambda.SQSEvents;

public class SQSRecordSet 
{
    public SQSEvent.SQSMessage[] Records { get; set; }
}