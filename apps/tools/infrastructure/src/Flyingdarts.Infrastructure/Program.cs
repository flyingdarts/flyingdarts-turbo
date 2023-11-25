using Amazon.CDK;
using Flyingdarts.Infrastructure.Constructs;
using Flyingdarts.Infrastructure.Constructs.v2;

namespace Flyingdarts.Infrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            new FlyingdartsStack(app, "Development", new StackProps
            {
                StackName = $"FD-Stack-Development",
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"), 
                    Region = System.Environment.GetEnvironmentVariable("AWS_REGION")
                }
            });
            
            new FlyingdartsStack(app, "Production", new StackProps
            {
                StackName = $"FD-Stack-Production",
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