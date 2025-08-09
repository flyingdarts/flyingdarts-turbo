namespace Flyingdarts.CDK.Constructs;

public class GithubActionsOIDCConstruct : Construct
{
    private OpenIdConnectProvider OIDCProvider { get; }
    private Role GithubOIDCRole { get; }
    public string Arn => GithubOIDCRole.RoleArn;

    // TODO: Refactor array away and just make a plain construct with props.repository
    public GithubActionsOIDCConstruct(
        Construct scope,
        string id,
        GithubActionsOIDCConstructProps props
    )
        : base(scope, id)
    {
        string githubDomain = "token.actions.githubusercontent.com";

        OIDCProvider = new OpenIdConnectProvider(
            this,
            props.GetResourceIdentifier(nameof(OIDCProvider)),
            new OpenIdConnectProviderProps
            {
                Url = $"https://{githubDomain}",
                ClientIds = ["sts.amazonaws.com"],
            }
        );

        GithubOIDCRole = new Role(
            this,
            props.GetResourceIdentifier(nameof(GithubOIDCRole)),
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
                                { $"{githubDomain}:aud", "sts.amazonaws.com" },
                            }
                        },
                        {
                            "StringLike",
                            new Dictionary<string, string>
                            {
                                { $"{githubDomain}:sub", $"repo:flyingdarts/{props.Repository}:*" },
                            }
                        },
                    }
                ),
                Path = "/github-actions/flyingdarts/",
                RoleName = $"{props.Repository}-deployment-role",
                InlinePolicies = new Dictionary<string, PolicyDocument>
                {
                    {
                        "allowAssumeOnAccount",
                        new PolicyDocument(
                            new PolicyDocumentProps
                            {
                                Statements =
                                [
                                    new PolicyStatement(
                                        new PolicyStatementProps
                                        {
                                            Effect = Effect.ALLOW,
                                            Actions =
                                            [
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
                                                "sts:AssumeRoleWithWebIdentity",
                                            ],
                                            Resources = ["*"],
                                        }
                                    ),
                                ],
                            }
                        )
                    },
                },
                Description = "This role is used for Github Actions to deploy functions to lambda",
                MaxSessionDuration = Duration.Hours(1),
            }
        );
    }
}
