
namespace Flyingdarts.Infrastructure;

public class AmazonStack : Stack
{
    public AmazonStack(Construct scope, IStackProps props) : base(scope, "Flyingdarts-Stack", props)
    {
        var communicationConstruct = new CommunicationConstruct(this, "Communication");

        var functions = new string[]
        {
            "Flyingdarts.Infrastructure",
            "Flyingdarts.Backend.Games.X01.JoinQueue",
            "Flyingdarts.Backend.Games.X01.Score",
            "Flyingdarts.Backend.Games.X01.Join",
            "Flyingdarts.Backend.Signalling.OnConnect",
            "Flyingdarts.Backend.Signalling.OnDefault",
            "Flyingdarts.Backend.Signalling.OnDisconnect",
            "Flyingdarts.Backend.User.Profile.Create",
            "Flyingdarts.Backend.User.Profile.Get",
            "Flyingdarts.Backend.User.Profile.Update",
            "Flyingdarts.Backend.User.Profile.VerifyEmail",
            "Flyingdarts.Backend.User.UpdateConnectionId"
        };

        var endpoints = functions
            .Where(x => 
                !x.Contains("Signalling") && 
                !x.Contains("Infrastructure") &&
                !x.Contains("VerifyEmail")
            ).ToArray();

        new BackendConstruct(this, "Backend", endpoints, new Dictionary<string, string>
        {
            {
                "QueueArn", communicationConstruct.Queue.QueueArn
            },
            {
                "QueueUrl", communicationConstruct.Queue.QueueUrl
            }
        });
        //new AmplifyConstruct(this, "Frontend");
        new AuthConstruct(this, "OIDC", functions);

    }
}
