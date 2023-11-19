using Amazon.CDK;
using Flyingdarts.Infrastructure.Constructs;

namespace Flyingdarts.Infrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new AmazonStack(app, new StackProps
            {
                StackName = "Flyingdarts-Stack-Development",
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"), 
                    Region = System.Environment.GetEnvironmentVariable("AWS_REGION")
                }
            });

            app.Synth();
        }
    }
}