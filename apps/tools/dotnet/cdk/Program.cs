using Amazon.CDK;
using Flyingdarts.CDK.Constructs;
using Flyingdarts.CDK.Constructs.v2;
using Environment = Amazon.CDK.Environment;

namespace Flyingdarts.CDK
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            new FlyingdartsStack(
                app,
                "Development",
                new StackProps
                {
                    StackName = $"FD-Stack-Development",
                    Env = new Environment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            new FlyingdartsStack(
                app,
                "Staging",
                new StackProps
                {
                    StackName = $"FD-Stack-Staging",
                    Env = new Environment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            new FlyingdartsStack(
                app,
                "Production",
                new StackProps
                {
                    StackName = $"FD-Stack-Production",
                    Env = new Environment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            new AuthStack(app, new StackProps { StackName = $"Auth-Stack" });

            app.Synth();
        }
    }
}
