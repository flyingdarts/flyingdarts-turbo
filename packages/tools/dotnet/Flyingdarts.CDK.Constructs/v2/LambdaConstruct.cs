namespace Flyingdarts.CDK.Constructs.v2;

public class LambdaConstruct : Construct
{
    public Function SignallingOnConnect { get; }
    public Function SignallingOnDefault { get; }
    public Function SignallingOnDisconnect { get; }
    public Function GamesX01Api { get; }
    public Function GamesX01Queue { get; }
    public Function ProfileApi { get; }
    public Function StatsApi { get; }
    private Function ProfileVerifyEmail { get; }
    public Function TournamentsApi { get; }
    public Function AuthLambda { get; }
    public LambdaConstruct(Construct scope, string id, string environment, DynamoDbConstruct dynamoDbConstruct,
        QueueConstruct queueConstruct) : base(scope, id)
    {
        if (dynamoDbConstruct is null)
            throw new Exception("Where the tables at?");

        #region Signalling functions

        SignallingOnConnect = new Function(this, $"Flyingdarts-Backend-Signalling-OnConnect-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Signalling-OnConnect-{environment}",
            Handler = "Flyingdarts.Backend.Signalling.OnConnect",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.SignallingTable.TableName },
            }
        });
        dynamoDbConstruct.SignallingTable.GrantFullAccess(SignallingOnConnect);

        SignallingOnDefault = new Function(this, $"Flyingdarts-Backend-Signalling-OnDefault-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Signalling-OnDefault-{environment}",
            Handler = "Flyingdarts.Backend.Signalling.OnDefault",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.SignallingTable.TableName },
            }
        });
        dynamoDbConstruct.SignallingTable.GrantFullAccess(SignallingOnDefault);

        SignallingOnDisconnect = new Function(this, $"Flyingdarts-Backend-Signalling-OnDisconnect-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Signalling-OnDisconnect-{environment}",
            Handler = "Flyingdarts.Backend.Signalling.OnDisconnect",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.SignallingTable.TableName },
            }
        });
        dynamoDbConstruct.SignallingTable.GrantFullAccess(SignallingOnDisconnect);

        #endregion

        #region X01 game functions

        GamesX01Api = new Function(this, $"Flyingdarts-Backend-Games-X01-Api-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Games-X01-Api-{environment}",
            Handler = "Flyingdarts.Backend.Games.X01.Api",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
            },
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[] { "ssm:GetParametersByPath", "dynamodb:*" },
                    Resources = new[]
                    {
                        "*"
                    }
                })
            }
        });
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(GamesX01Api);

        GamesX01Queue = new Function(this, $"Flyingdarts-Backend-Games-X01-Queue-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Games-X01-Queue-{environment}",
            Handler = "Flyingdarts.Backend.Games.X01.Queue",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
            },
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[] { "ssm:GetParametersByPath", "dynamodb:*" },
                    Resources = new[]
                    {
                        "*"
                    }
                })
            }
        });
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(GamesX01Queue);
        dynamoDbConstruct.X01QueueTable.GrantStreamRead(GamesX01Queue);
        // Add DynamoDB stream trigger to Lambda
        GamesX01Queue.AddEventSource(new DynamoEventSource(dynamoDbConstruct.X01QueueTable, new DynamoEventSourceProps
        {
            BatchSize = 100, // Set the batch size as needed
            StartingPosition = StartingPosition.TRIM_HORIZON, // Set the starting position as needed
        }));
        #endregion
        StatsApi = new Function(this, $"Flyingdarts-Backend-Stats-Api-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Stats-Api-{environment}",
            Handler = "Flyingdarts.Backend.Stats.Api",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
            },
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[] { "ssm:GetParametersByPath", "dynamodb:DescribeTable", "dynamodb:BatchWriteItem" },
                    Resources = new[]
                    {
                        "*"
                    }
                })
            }
        });
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(StatsApi);
        
        #region User profile functions

        ProfileApi = new Function(this, $"Flyingdarts-Backend-User-Profile-Api-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-User-Profile-Api-{environment}",
            Handler = "Flyingdarts.Backend.User.Profile.Api",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "SqsQueueUrl", queueConstruct.VerifyEmailQueue.QueueUrl },
            },
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[] { "ssm:GetParametersByPath", "dynamodb:DescribeTable", "dynamodb:BatchWriteItem" },
                    Resources = new[]
                    {
                        "*"
                    }
                }),
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[]
                    {
                        "sqs:SendMessage"
                    },
                    Resources = new[]
                    {
                        queueConstruct.VerifyEmailQueue.QueueArn
                    }
                })
            }
        });
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(ProfileApi);

        ProfileVerifyEmail = new Function(this, $"Flyingdarts-Backend-User-Profile-VerifyEmail-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-User-Profile-VerifyEmail-{environment}",
            Handler = "Flyingdarts.Backend.User.Profile.VerifyEmail",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
            },
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[] { "ssm:GetParametersByPath", "dynamodb:*" },
                    Resources = new[]
                    {
                        "*"
                    }
                })
            }
        });
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(ProfileVerifyEmail);

        #endregion

        #region Tournament functions

        TournamentsApi = new Function(this, $"Flyingdarts-Backend-Tournaments-Api-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Tournaments-Api-{environment}",
            Handler = "Flyingdarts.Backend.Tournaments.Api",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[] { "ssm:GetParametersByPath", "dynamodb:*" },
                    Resources = new[]
                    {
                        "*"
                    }
                })
            }
        });
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(TournamentsApi);

        #endregion

        AuthLambda = new Function(this, $"Flyingdarts-Backend-Auth-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Auth-{environment}",
            Handler = "Flyingdarts.Backend.Auth",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet8"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            InitialPolicy = new[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Actions = new[] { "cognito-idp:ListUsers"},
                    Resources = new[]
                    {
                        "*"
                    }
                })
            },
            Environment = new Dictionary<string, string>
            {
                {
                    "AuthressApiBasePath", System.Environment.GetEnvironmentVariable("AuthressApiBasePath")!
                },
                {
                    "AuthressResourceGroupId", System.Environment.GetEnvironmentVariable("AuthressResourceGroupId")!
                }
            }
        });
    }
}