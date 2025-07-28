namespace Flyingdarts.CDK.Constructs.v2;

public class LambdaConstruct : Construct
{
    public Function SignallingApi { get; }
    public Function GamesX01Api { get; }
    public Function FriendsApi { get; }

    public Function AuthLambda { get; }

    public LambdaConstruct(Construct scope, string id, string environment, DynamoDbConstruct dynamoDbConstruct)
        : base(scope, id)
    {
        if (dynamoDbConstruct is null)
            throw new Exception("Where the tables at?");

        #region Signalling API

        SignallingApi = new Function(
            this,
            $"Flyingdarts-Backend-Signalling-Api-{environment}",
            new FunctionProps
            {
                FunctionName = $"Flyingdarts-Backend-Signalling-Api-{environment}",
                Handler = "Flyingdarts.Backend.Signalling.Api",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string> { { "TableName", dynamoDbConstruct.ApplicationTable.TableName } },
                InitialPolicy = new[]
                {
                    new PolicyStatement(
                        new PolicyStatementProps { Actions = new[] { "ssm:GetParametersByPath", "dynamodb:*" }, Resources = new[] { "*" } }
                    ),
                },
            }
        );
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(SignallingApi);

        #endregion

        GamesX01Api = new Function(
            this,
            $"Flyingdarts-Backend-Games-X01-Api-{environment}",
            new FunctionProps
            {
                FunctionName = $"Flyingdarts-Backend-Games-X01-Api-{environment}",
                Handler = "Flyingdarts.Backend.Games.X01.Api",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string> { { "TableName", dynamoDbConstruct.ApplicationTable.TableName } },
                InitialPolicy = new[]
                {
                    new PolicyStatement(
                        new PolicyStatementProps { Actions = new[] { "ssm:GetParametersByPath", "dynamodb:*" }, Resources = new[] { "*" } }
                    ),
                },
            }
        );
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(GamesX01Api);

        FriendsApi = new Function(
            this,
            $"Flyingdarts-Backend-Friends-Api-{environment}",
            new FunctionProps
            {
                FunctionName = $"Flyingdarts-Backend-Friends-Api-{environment}",
                Handler = "Flyingdarts.Backend.Friends.Api",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string> { { "TableName", dynamoDbConstruct.ApplicationTable.TableName } },
                InitialPolicy = new[]
                {
                    new PolicyStatement(
                        new PolicyStatementProps
                        {
                            Actions = new[] { "dynamodb:DescribeTable", "dynamodb:BatchWriteItem", "ssm:GetParametersByPath" },
                            Resources = new[] { "*" },
                        }
                    ),
                },
            }
        );
        dynamoDbConstruct.ApplicationTable.GrantFullAccess(FriendsApi);

        AuthLambda = new Function(
            this,
            $"Flyingdarts-Backend-Auth-{environment}",
            new FunctionProps
            {
                FunctionName = $"Flyingdarts-Backend-Auth-{environment}",
                Handler = "Flyingdarts.Backend.Auth",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                InitialPolicy = new[]
                {
                    new PolicyStatement(
                        new PolicyStatementProps { Actions = new[] { "cognito-idp:ListUsers" }, Resources = new[] { "*" } }
                    ),
                },
                Environment = new Dictionary<string, string>
                {
                    { "AuthressApiBasePath", System.Environment.GetEnvironmentVariable("AuthressApiBasePath")! },
                    { "AuthressResourceGroupId", System.Environment.GetEnvironmentVariable("AuthressResourceGroupId")! },
                },
            }
        );
    }
}
