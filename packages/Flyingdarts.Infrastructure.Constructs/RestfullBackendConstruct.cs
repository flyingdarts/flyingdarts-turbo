using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Function = Amazon.CDK.AWS.Lambda.Function;
using FunctionProps = Amazon.CDK.AWS.Lambda.FunctionProps;
using IResource = Amazon.CDK.AWS.APIGateway.IResource;
using Stage = Amazon.CDK.AWS.APIGateway.Stage;
using StageProps = Amazon.CDK.AWS.APIGateway.StageProps;

public class RestfullBackendConstruct : Construct
{
    private RestApi RestApi { get; }
    private Deployment Deployment { get; }
    private Stage Stage { get; }
    public RestfullBackendConstruct(Construct scope, string id) : base(scope, id)
    {
        CreateFunction(
            "Flyingdarts-RestfullBackend-User-Profile-Get", 
            "Flyingdarts-Application-Table"
        );
    }
    private Function CreateFunction(string functionName, string tableName)
    {
        var lambda = new Function(this, functionName.Replace(".", "-"), new FunctionProps
        {
            FunctionName = functionName.Replace(".", "-"),
            Handler = functionName,
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                {
                    "TableName", tableName
                }
            },
            InitialPolicy = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Actions = new [] { 
                            "ssm:GetParametersByPath",
                            "dynamodb:DescribeTable", 
                            "dynamodb:Query" 
                        },
                        Resources = new []
                        {
                            "*"
                        }
                    })
                }
        });

        lambda.AddEnvironment("LAMBDA_NET_SERIALIZER_DEBUG", "true");
        lambda.AddEnvironment("EnvironmentName", "Development");

        return lambda;
    }
}