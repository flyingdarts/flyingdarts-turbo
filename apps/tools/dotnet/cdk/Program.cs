using Amazon.CDK;
using Amazon.CDK.AWS.CloudFormation;
using Flyingdarts.CDK.Constructs;
using StackEnvironment = Amazon.CDK.Environment;

namespace Flyingdarts.CDK
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var domainResources = new DomainStack(
                app,
                new DomainStackProps
                {
                    DeploymentEnvironment = DeploymentEnvironment.None, // This stack is imported from existing resources
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            // Frontend stack - Development
            new FrontendStack(
                app,
                new FrontendStackProps
                {
                    DeploymentEnvironment = DeploymentEnvironment.Development,
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                    HostedZone = domainResources.FlyingdartsHostedZone,
                    Certificate = domainResources.FlyingdartsCertificate,
                }
            );

            // Frontend stack - Staging
            new FrontendStack(
                app,
                new FrontendStackProps
                {
                    DeploymentEnvironment = DeploymentEnvironment.Staging,
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                    HostedZone = domainResources.FlyingdartsHostedZone,
                    Certificate = domainResources.FlyingdartsCertificate,
                }
            );

            // Frontend stack - Production
            new FrontendStack(
                app,
                new FrontendStackProps
                {
                    DeploymentEnvironment = DeploymentEnvironment.Production,
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                    HostedZone = domainResources.FlyingdartsHostedZone,
                    Certificate = domainResources.FlyingdartsCertificate,
                }
            );

            // Backend stack - Development
            new BackendStack(
                app,
                new BackendStackProps
                {
                    DeploymentEnvironment = DeploymentEnvironment.Development,
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            // Backend stack - Staging
            new BackendStack(
                app,
                new BackendStackProps
                {
                    DeploymentEnvironment = DeploymentEnvironment.Staging,
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            // Backend stack - Production
            new BackendStack(
                app,
                new BackendStackProps
                {
                    DeploymentEnvironment = DeploymentEnvironment.Production,
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            // Auth stack
            new AuthStack(
                app,
                new AuthStackProps
                {
                    Repository = "flyingdarts-turbo",
                    DeploymentEnvironment = DeploymentEnvironment.None,
                    StackEnvironment = new StackEnvironment
                    {
                        Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT"),
                        Region = System.Environment.GetEnvironmentVariable("AWS_REGION"),
                    },
                }
            );

            app.Synth();
        }
    }
}
