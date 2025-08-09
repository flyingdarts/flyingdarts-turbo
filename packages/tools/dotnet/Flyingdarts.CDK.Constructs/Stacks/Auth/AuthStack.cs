using System.Reflection.Metadata;

namespace Flyingdarts.CDK.Constructs;

public class AuthStack : Stack
{
    private GithubActionsOIDCConstruct GithubActionsOIDC { get; }
    private string GithubActionsRoleArn => GithubActionsOIDC.Arn;

    public AuthStack(Construct scope, AuthStackProps props)
        : base(scope, props.StackId, new StackProps { Env = props.StackEnvironment })
    {
        GithubActionsOIDC = new GithubActionsOIDCConstruct(
            this,
            props.GetUniqueResourceId(nameof(GithubActionsOIDC)),
            new GithubActionsOIDCConstructProps
            {
                Repository = props.Repository,
                DeploymentEnvironment = props.DeploymentEnvironment,
            }
        );

        new CfnOutput(
            this,
            props.GetUniqueResourceId($"{nameof(GithubActionsRoleArn)}-CfnOutput"),
            new CfnOutputProps
            {
                ExportName = nameof(GithubActionsRoleArn),
                Value = GithubActionsRoleArn,
            }
        );
    }
}
