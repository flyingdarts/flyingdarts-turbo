namespace Flyingdarts.CDK.Constructs;

public interface IWebSocketApiFunctions
{
    public Function Signalling { get; }
    public Function Backend { get; }
}

public interface IRestApiFunctions
{
    public Function Friends { get; }
}

public interface RequestAuthorizerFunction
{
    // TODO:  Implement the rust authorizer again
    public Function Authorizer { get; }
}

public class LambdaConstruct
    : BaseConstruct<LambdaConstructProps>,
        IWebSocketApiFunctions,
        IRestApiFunctions,
        RequestAuthorizerFunction
{
    public Function Signalling { get; }
    public Function Backend { get; }
    public Function Friends { get; }
    public Function Authorizer { get; }

    public LambdaConstruct(Construct scope, LambdaConstructProps props)
        : base(scope, props)
    {
        if (props.DynamoDbConstruct is null)
            throw new Exception("Where the tables at?");

        Signalling = new Function(
            this,
            props.GetResourceIdentifier(nameof(Signalling)),
            new FunctionProps
            {
                FunctionName = GetUniqueApiName(nameof(Signalling)),
                Handler = "Flyingdarts.Backend.Signalling.Api",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string>
                {
                    { "TableName", props.DynamoDbConstruct.Application.TableName },
                    {
                        "DyteAuthorizationHeaderValue",
                        $"Basic {System.Environment.GetEnvironmentVariable("DyteAuthorizationHeaderValue")!}"
                    },
                },
                InitialPolicy =
                [
                    new PolicyStatement(
                        new PolicyStatementProps
                        {
                            Actions = ["ssm:GetParametersByPath", "dynamodb:*"],
                            Resources = ["*"],
                        }
                    ),
                ],
            }
        );
        props.DynamoDbConstruct.Application.GrantFullAccess(Signalling);

        Backend = new Function(
            this,
            props.GetResourceIdentifier(nameof(Backend)),
            new FunctionProps
            {
                FunctionName = GetUniqueApiName(nameof(Backend)),
                Handler = "Flyingdarts.Backend.Api",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string>
                {
                    { "TableName", props.DynamoDbConstruct.Application.TableName },
                    {
                        "DyteAuthorizationHeaderValue",
                        $"Basic {System.Environment.GetEnvironmentVariable("DyteAuthorizationHeaderValue")!}"
                    },
                },
                InitialPolicy =
                [
                    new PolicyStatement(
                        new PolicyStatementProps
                        {
                            Actions = ["ssm:GetParametersByPath", "dynamodb:*"],
                            Resources = ["*"],
                        }
                    ),
                ],
            }
        );
        props.DynamoDbConstruct.Application.GrantFullAccess(Backend);

        Friends = new Function(
            this,
            props.GetResourceIdentifier(nameof(Friends)),
            new FunctionProps
            {
                FunctionName = GetUniqueApiName(nameof(Friends)),
                Handler = "Flyingdarts.Backend.Friends.Api",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                Environment = new Dictionary<string, string>
                {
                    { "TableName", props.DynamoDbConstruct.Application.TableName },
                },
                InitialPolicy =
                [
                    new PolicyStatement(
                        new PolicyStatementProps
                        {
                            Actions =
                            [
                                "dynamodb:DescribeTable",
                                "dynamodb:BatchWriteItem",
                                "ssm:GetParametersByPath",
                            ],
                            Resources = ["*"],
                        }
                    ),
                ],
            }
        );
        props.DynamoDbConstruct.Application.GrantFullAccess(Friends);

        Authorizer = new Function(
            this,
            props.GetResourceIdentifier(nameof(Authorizer)),
            new FunctionProps
            {
                FunctionName = GetUniqueName(nameof(Authorizer)),
                Handler = "Flyingdarts.Backend.Auth",
                Code = Code.FromAsset("lambda.zip"),
                Runtime = new Runtime("dotnet8"),
                Timeout = Duration.Seconds(30),
                MemorySize = 256,
                InitialPolicy =
                [
                    new PolicyStatement(
                        new PolicyStatementProps
                        {
                            Actions = ["cognito-idp:ListUsers"],
                            Resources = ["*"],
                        }
                    ),
                ],
                Environment = new Dictionary<string, string>
                {
                    {
                        "AuthressApiBasePath",
                        System.Environment.GetEnvironmentVariable("AuthressApiBasePath")!
                    },
                    {
                        "AuthressResourceGroupId",
                        System.Environment.GetEnvironmentVariable("AuthressResourceGroupId")!
                    },
                },
            }
        );
    }
}
