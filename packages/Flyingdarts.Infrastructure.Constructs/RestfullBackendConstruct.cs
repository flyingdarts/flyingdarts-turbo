using Amazon.CDK.AWS.APIGateway;
using IResource = Amazon.CDK.AWS.APIGateway.IResource;
using Stage = Amazon.CDK.AWS.APIGateway.Stage;
using StageProps = Amazon.CDK.AWS.APIGateway.StageProps;

public class RestfullBackendConstruct : Construct
{
    private RestApi RestApi { get; }
    private Deployment Deployment { get; }
    private Stage Stage { get; }
    public RestfullBackendConstruct(Construct scope, string id, string[] repositories) : base(scope, id)
    {
        RestApi = new RestApi(this, "Flyingdarts-RestfullBackend-Api", new RestApiProps
        {
            RestApiName = "Flyingdarts-RestfullBackend-Api",
        });

        Deployment = new Deployment(this, "FlyingdartsRestfullBackendStageDeployment", new DeploymentProps
        { 
            Api = RestApi 
        });

        Stage = new Stage(this, "Flyingdarts-RestfullBackend-Api-Stage", new StageProps
        {
            Deployment = Deployment,
            StageName = "Development"
        });

        var v1 = RestApi.Root.AddResource("v1");
        var user = v1.AddResource("user");
        var userRepositories = repositories.Where(x => x.Contains("User"));


        userRepositories.ToList().ForEach((x) =>
        {
            var function = CreateFunction(x.Replace(".", "-"), "Flyingdarts-Application-Table");

            var routeParts = x.Split(".");

            // Remove Flyingdarts.RestfullBackend.Entity
            routeParts = routeParts.TakeLast(routeParts.Count() - 3).ToArray();

            // Take last entry as method and toUpper it
            var method = routeParts.Last().ToUpper();

            // Remove last entry
            routeParts = routeParts.Take(routeParts.Count() - 1).ToArray();

            // Create route hierarchy
            var route = CreateResourceHierarchy(user, routeParts.ToList());

            // Add method to route.
            route.AddMethod(method, new LambdaIntegration(function));
        });

    }
    private static IResource CreateResourceHierarchy(IResource parentResource, List<string> routeParts)
    {
        if (routeParts.Any())
        {
            var routePart = routeParts.First();
            routeParts.RemoveAt(0);

            var resource = parentResource.AddResource(routePart);
            return CreateResourceHierarchy(resource, routeParts);
        }

        return parentResource;
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
                        Actions = new [] { "ssm:GetParametersByPath" },
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