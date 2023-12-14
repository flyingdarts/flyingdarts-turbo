using Flyingdarts.Infrastructure.Constructs.v2;

public class LambdaConstruct : Construct
{
    public Function SignallingOnConnect { get; }
    public Function SignallingOnDefault { get; }
    public Function SignallingOnDisconnect { get; }
    public Function GamesX01Create { get;  }
    public Function GamesX01Join { get; }
    public Function GamesX01JoinQueue { get; }
    public Function GamesX01Queue { get; }
    public Function GamesX01Score { get; }
    public Function ProfileCreate { get; }
    public Function ProfileGet { get; }
    public Function ProfileUpdate { get; }
    public Function ProfileVerifyEmail { get; }
    public Function TournamentsCreate { get; }

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
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.SignallingTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
            }
        });
        dynamoDbConstruct.SignallingTable.GrantFullAccess(SignallingOnConnect);

        SignallingOnDefault = new Function(this, $"Flyingdarts-Backend-Signalling-OnDefault-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Signalling-OnDefault-{environment}",
            Handler = "Flyingdarts.Backend.Signalling.OnDefault",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.SignallingTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
            }
        });
        dynamoDbConstruct.SignallingTable.GrantFullAccess(SignallingOnDefault);

        SignallingOnDisconnect = new Function(this, $"Flyingdarts-Backend-Signalling-OnDisconnect-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Signalling-OnDisconnect-{environment}",
            Handler = "Flyingdarts.Backend.Signalling.OnDisconnect",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.SignallingTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
            }
        });
        dynamoDbConstruct.SignallingTable.GrantFullAccess(SignallingOnDisconnect);

        #endregion

        #region X01 game functions

        GamesX01Create= new Function(this, $"Flyingdarts-Backend-Games-X01-Create-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Games-X01-Create-{environment}",
            Handler = "Flyingdarts.Backend.Games.X01.Create",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(GamesX01Create);
        
        GamesX01Join = new Function(this, $"Flyingdarts-Backend-Games-X01-Join-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Games-X01-Join-{environment}",
            Handler = "Flyingdarts.Backend.Games.X01.Join",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(GamesX01Join);

        GamesX01JoinQueue = new Function(this, $"Flyingdarts-Backend-Games-X01-JoinQueue-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Games-X01-JoinQueue-{environment}",
            Handler = "Flyingdarts.Backend.Games.X01.JoinQueue",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(GamesX01JoinQueue);
        
        GamesX01Queue = new Function(this, $"Flyingdarts-Backend-Games-X01-Queue-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Games-X01-Queue-{environment}",
            Handler = "Flyingdarts.Backend.Games.X01.Queue",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        
        GamesX01Score = new Function(this, $"Flyingdarts-Backend-Games-X01-Score-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Games-X01-Score-{environment}",
            Handler = "Flyingdarts.Backend.Games.X01.Score",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(GamesX01Score);

        #endregion

        #region User profile functions

        ProfileCreate = new Function(this, $"Flyingdarts-Backend-User-Profile-Create-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-User-Profile-Create-{environment}",
            Handler = "Flyingdarts.Backend.User.Profile.Create",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(ProfileCreate);

        ProfileGet = new Function(this, $"Flyingdarts-Backend-User-Profile-Get-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-User-Profile-Get-{environment}",
            Handler = "Flyingdarts.Backend.User.Profile.Get",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(ProfileGet);

        ProfileUpdate = new Function(this, $"Flyingdarts-Backend-User-Profile-Update-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-User-Profile-Update-{environment}",
            Handler = "Flyingdarts.Backend.User.Profile.Update",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "SqsQueueUrl", queueConstruct.VerifyEmailQueue.QueueUrl },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(ProfileUpdate);

        ProfileVerifyEmail = new Function(this, $"Flyingdarts-Backend-User-Profile-VerifyEmail-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-User-Profile-VerifyEmail-{environment}",
            Handler = "Flyingdarts.Backend.User.Profile.VerifyEmail",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        
        TournamentsCreate = new Function(this, $"Flyingdarts-Backend-Tournaments-Create-{environment}", new FunctionProps
        {
            FunctionName = $"Flyingdarts-Backend-Tournaments-Create-{environment}",
            Handler = "Flyingdarts.Backend.Tournaments-Create",
            Code = Code.FromAsset("lambda.zip"),
            Runtime = new Runtime("dotnet6"),
            Timeout = Duration.Seconds(30),
            MemorySize = 256,
            Environment = new Dictionary<string, string>
            {
                { "TableName", dynamoDbConstruct.ApplicationTable.TableName },
                { "LAMBDA_NET_SERIALIZER_DEBUG", "true" },
                { "EnvironmentName", "Development" }
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
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(TournamentsCreate);
        
        #endregion
    }
}