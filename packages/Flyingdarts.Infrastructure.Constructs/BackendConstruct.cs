using Amazon.CDK.AWS.SQS;
using Flyingdarts.Shared;

public class BackendConstruct : Construct
{
    private Table SignallingTable { get; }
    private WebSocketApi Api { get; }
    private WebSocketStage Stage { get; }
    private Table ApplicationTable { get; }
    public BackendConstruct(Construct scope, string id, string[] repositories, Dictionary<string, string> stuff) : base(scope, id)
    {
        // Setup the signalling table and functions
        SignallingTable = new Table(this, "Flyingdarts-Signalling-Table", new TableProps
        {
            TableName = "Flyingdarts-Signalling-Table",
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            BillingMode = BillingMode.PAY_PER_REQUEST,
            Encryption = TableEncryption.AWS_MANAGED,
            PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "ConnectionId" },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        // Create Lambda Functions
        var OnConnect = CreateSignallingFunction("Flyingdarts.Backend.Signalling.OnConnect", SignallingTable);
        var OnDefault = CreateSignallingFunction("Flyingdarts.Backend.Signalling.OnDefault", SignallingTable);
        var OnDisconnect = CreateSignallingFunction("Flyingdarts.Backend.Signalling.OnDisconnect", SignallingTable);

        // Grant lambda functions access to the SignallingTable
        SignallingTable.GrantFullAccess(OnConnect);
        SignallingTable.GrantFullAccess(OnDefault);
        SignallingTable.GrantFullAccess(OnDisconnect);

        // Setup the WebSocketApi and Signalling integrations
        Api = new WebSocketApi(this, "Flyingdarts-Backend-Api", new WebSocketApiProps
        {
            ApiName = "Flyingdarts.Backend.Api",
            ConnectRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration("Flyingdarts-Backend-Api-OnConnect-Integration", OnConnect)
            },
            DefaultRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration("Flyingdarts-Backend-Api-OnDefault-Integration", OnDefault)
            },
            DisconnectRouteOptions = new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration("Flyingdarts-Backend-Api-OnDisconnect-Integration", OnDisconnect)
            }
        });

        // Create a development stage
        Stage = new WebSocketStage(this, "Stage", new WebSocketStageProps
        {
            WebSocketApi = Api,
            StageName = "Development",
            AutoDeploy = true
        });


        OnDefault.AddEnvironment("WebSocketApiUrl", Stage.Url);
        Stage.GrantManagementApiAccess(OnDefault);


        ApplicationTable = new Table(this, "Flyingdarts-Application-Table", new TableProps
        {
            TableName = "Flyingdarts-Application-Table",
            Stream = StreamViewType.NEW_AND_OLD_IMAGES,
            BillingMode = BillingMode.PAY_PER_REQUEST,
            Encryption = TableEncryption.AWS_MANAGED,
            PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
            SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
            RemovalPolicy = RemovalPolicy.DESTROY,
        });

        ApplicationTable.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "LSI1",
            SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
        });

        foreach (var functionName in repositories)
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
                        "TableName", ApplicationTable.TableName
                    }
                },
                InitialPolicy = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Actions = new [] { "ssm:GetParametersByPath","dynamodb:DescribeTable" },
                        Resources = new []
                        {
                            "*"
                        }
                    })
                }
            });

            if (functionName == "Flyingdarts.Backend.User.Profile.Update")
            {
                lambda.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[]
                    {
                        "sqs:SendMessage"
                    },
                    Resources = new[]
                    {
                        stuff["QueueArn"]
                    }
                }));
                lambda.AddEnvironment("SqsQueueUrl", stuff["QueueUrl"]);
            }

            var pathComponents = functionName.Split(".");
            pathComponents = pathComponents.TakeLast(pathComponents.Length - 2).ToArray();
            var routeKey = string.Join("/", pathComponents.Prepend("v2")).ToLower();
            var integrationId = string.Join("-", pathComponents.Append("ApiGateway-Integration"));


            Api.AddRoute(routeKey, new WebSocketRouteOptions
            {
                Integration = new WebSocketLambdaIntegration(integrationId, lambda),
                ReturnResponse = true // Every Route in the Core should send its response to the client.
            });

            Stage.GrantManagementApiAccess(lambda);

            ApplicationTable.GrantFullAccess(lambda);

            lambda.AddEnvironment("LAMBDA_NET_SERIALIZER_DEBUG", "true");
            lambda.AddEnvironment("EnvironmentName", "Development");
            lambda.AddEnvironment("WebSocketApiUrl", Stage.Url);
        }


        new CfnOutput(this, "WebSocketUrlCfnOutput", new CfnOutputProps
        {
            ExportName = "WebSocketApiUrl",
            Value = Stage.Url
        });

        // Create a parameter in the Systems Manager Parameter Store
        new StringParameter(this, "MyParameter", new StringParameterProps
        {
            StringValue = ApplicationTable.TableName, // Set the value for the parameter
            ParameterName = $"/{System.Environment.GetEnvironmentVariable("EnvironmentName")}/Application/DynamoDb"
        });
    }

    private Function CreateSignallingFunction(string functionName, Table table)
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
                    "TableName", table.TableName
                }
            }
        });

        lambda.AddEnvironment("LAMBDA_NET_SERIALIZER_DEBUG", "true");

        return lambda;
    }
}