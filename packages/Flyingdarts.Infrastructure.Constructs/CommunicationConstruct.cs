using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.SSM;
using Flyingdarts.Shared;

public class CommunicationConstruct : Construct
{
    public Queue Queue;
    public Function Function;
    public CommunicationConstruct(Construct scope, string id) : base(scope, id)
    {
        Queue = new Queue(this, Lambdas.Functions.Utilities.Emails.VERIFY_USER_EMAIL_QUEUE.Replace(".", "-"), new QueueProps
        {
            QueueName = Lambdas.Functions.Utilities.Emails.VERIFY_USER_EMAIL_QUEUE.Replace(".", string.Empty),
            RetentionPeriod = Duration.Seconds(60)
        });

        Function = new Function(this, Lambdas.Functions.Utilities.Emails.VERIFY_USER_EMAIL.Replace(".", "-"), new FunctionProps
        {
            FunctionName = Lambdas.Functions.Utilities.Emails.VERIFY_USER_EMAIL.Replace(".", "-"),
            Handler = Lambdas.Functions.Utilities.Emails.VERIFY_USER_EMAIL,
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new []
                    {
                        "ses:SendEmail",
                        "ses:SendRawEmail"
                    },
                    Resources = new []
                    {
                        "*"
                    }
                })
            }
        });

        Function.AddEnvironment("LAMBDA_NET_SERIALIZER_DEBUG", "true");

        Function.AddEventSource(new SqsEventSource(Queue, new SqsEventSourceProps
        {
            BatchSize = 1,
            Enabled = true,
            MaxConcurrency = 2
        }));
    }
}