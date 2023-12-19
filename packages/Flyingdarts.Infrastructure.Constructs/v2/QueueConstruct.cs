namespace Flyingdarts.Infrastructure.Constructs.v2;

public class QueueConstruct : Construct
{
    public Queue VerifyEmailQueue { get; }
    public Queue X01GameQueue { get; }

    public QueueConstruct(Construct scope, string id, string environment) : base(scope, id)
    {
        VerifyEmailQueue = new Queue(this, $"Flyingdarts-Backend-User-Profile-VerifyEmail-Queue-{environment}",
            new QueueProps
            {
                QueueName = $"FlyingdartsBackendUserProfileVerifyEmailQueue{environment}",
                RetentionPeriod = Duration.Seconds(60)
            });
    }
}