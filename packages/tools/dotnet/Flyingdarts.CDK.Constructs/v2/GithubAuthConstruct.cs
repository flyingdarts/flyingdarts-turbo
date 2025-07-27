namespace Flyingdarts.CDK.Constructs.v2;

public class GithubAuthConstruct : Construct
{
    public OpenIdConnectProvider OIDCProvider { get; }

    public GithubAuthConstruct(Construct scope, string id, string[] repositories)
        : base(scope, id)
    {
        string githubDomain = "token.actions.githubusercontent.com";

        OIDCProvider = new OpenIdConnectProvider(
            this,
            $"GithubOIDCProvider".Replace(".", string.Empty),
            new OpenIdConnectProviderProps
            {
                Url = $"https://{githubDomain}",
                ClientIds = new string[] { "sts.amazonaws.com" },
            }
        );

        foreach (var repository in repositories)
        {
            var role = new Role(
                this,
                $"{repository}.Github.OIDC.Role".Replace(".", string.Empty),
                new RoleProps
                {
                    AssumedBy = new WebIdentityPrincipal(
                        OIDCProvider.OpenIdConnectProviderArn,
                        new Dictionary<string, object>
                        {
                            {
                                "StringEquals",
                                new Dictionary<string, string>
                                {
                                    { $"{githubDomain}:aud", "sts.amazonaws.com" }
                                }
                            },
                            {
                                "StringLike",
                                new Dictionary<string, string>
                                {
                                    { $"{githubDomain}:sub", $"repo:flyingdarts/{repository}:*" }
                                }
                            }
                        }
                    ),
                    Path = "/github-actions/flyingdarts/",
                    RoleName = $"{repository}-deployment-role",
                    InlinePolicies = new Dictionary<string, PolicyDocument>
                    {
                        {
                            "allowAssumeOnAccount",
                            new PolicyDocument(
                                new PolicyDocumentProps
                                {
                                    Statements = new PolicyStatement[]
                                    {
                                        new PolicyStatement(
                                            new PolicyStatementProps
                                            {
                                                Effect = Effect.ALLOW,
                                                Actions = new string[]
                                                {
                                                    "iam:ListRoles",
                                                    "s3:ListBucket",
                                                    "s3:GetObject",
                                                    "s3:PutObject",
                                                    "s3:DeleteObject",
                                                    "lambda:GetFunctionConfiguration",
                                                    "lambda:UpdateFunctionConfiguration",
                                                    "lambda:UpdateFunctionCode",
                                                    "cloudfront:CreateInvalidation",
                                                    "sts:AssumeRole",
                                                    "sts:AssumeRoleWithWebIdentity"
                                                },
                                                Resources = new string[] { "*" }
                                            }
                                        )
                                    }
                                }
                            )
                        }
                    },
                    Description =
                        "This role is used for Github Actions to deploy functions to lambda",
                    MaxSessionDuration = Duration.Hours(1)
                }
            );

            new CfnOutput(
                scope,
                $"{repository.Replace(".", string.Empty)}CfnOutput",
                new CfnOutputProps
                {
                    ExportName = $"{repository.Replace(".", string.Empty)}GithubOIDCRoleArn",
                    Value = role.RoleArn
                }
            );
        }
    }
}
