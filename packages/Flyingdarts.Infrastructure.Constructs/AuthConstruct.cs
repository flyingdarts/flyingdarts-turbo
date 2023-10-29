
public class AuthConstruct : Construct
{
    public OpenIdConnectProvider OIDCProvider { get; }
    public AuthConstruct(Construct scope, string id, string[] repositories) : base(scope, id)
    {
        string githubDomain = "token.actions.githubusercontent.com";

        OIDCProvider = new OpenIdConnectProvider(this, $"GithubOIDCProvider".Replace(".", string.Empty),
            new OpenIdConnectProviderProps
            {
                Url = $"https://{githubDomain}",
                ClientIds = new string[] { "sts.amazonaws.com" },
            });

        foreach (var repository in repositories)
        {
            var role = new Role(this, $"{repository}.Github.OIDC.Role".Replace(".", string.Empty), new RoleProps
            {
                AssumedBy = new WebIdentityPrincipal(OIDCProvider.OpenIdConnectProviderArn, new Dictionary<string, object>
                {
                    {
                        "ForAllValues:StringEquals", new Dictionary<string, string> {
                            { $"{githubDomain}:sub",  $"repo:flyingdarts/{repository}:ref:refs/heads/main" }
                        }
                    }
                }),
                Path = $"/github-actions/flyingdarts/",
                RoleName = $"{repository}-deployment-role",
                InlinePolicies = new Dictionary<string, PolicyDocument>
                {
                    {
                        "allowAssumeOnAccountB", new PolicyDocument(new PolicyDocumentProps {
                            Statements = new PolicyStatement[] {
                                new PolicyStatement(new PolicyStatementProps {
                                    Effect = Effect.ALLOW,
                                    Actions = new string[] { "sts:AssumeRoleWithWebIdentity" },
                                    Resources = new string[] { $"arn:aws:iam::{System.Environment.GetEnvironmentVariable("AWS_ACCOUNT")}:role/*" }
                                }),
                                new PolicyStatement(new PolicyStatementProps {
                                    Effect = Effect.ALLOW,
                                    Actions =
                                        repository == "Flyingdarts.Infrastructure"
                                            ? new string[]
                                            {
                                                "cloudformation:DescribeStacks"
                                            }
                                            : new string[] {
                                                "iam:ListRoles",
                                                "lambda:GetFunctionConfiguration",
                                                "lambda:UpdateFunctionConfiguration",
                                                "lambda:UpdateFunctionCode"
                                            },
                                    Resources = new string[] { "*" }
                                })
                            }
                        })
                    }
                },
                Description = "This role is used for Github Actions to deploy functions to lambda",
                MaxSessionDuration = Duration.Hours(1)
            });

            new CfnOutput(scope, $"{repository.Replace(".", string.Empty)}CfnOutput", new CfnOutputProps
            {
                ExportName = $"{repository.Replace(".", string.Empty)}GithubOIDCRoleArn",
                Value = role.RoleArn
            });
        }
    }
}