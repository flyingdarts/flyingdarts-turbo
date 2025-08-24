using Amazon.CDK.AWS.Route53.Targets;

namespace Flyingdarts.CDK.Constructs;

public class FrontendStack : BaseStack<FrontendStackProps>
{
    private WebApplicationHostingConstruct WebApp { get; }
    private CnameRecord CnameRecord { get; }
    private ARecord? ARecord { get; }
    private string DomainName => CnameRecord.DomainName;

    public FrontendStack(Construct scope, FrontendStackProps props)
        : base(scope, props)
    {
        WebApp = new WebApplicationHostingConstruct(
            this,
            props.GetUniqueResourceId(nameof(WebApp)),
            new WebApplicationHostingConstructProps
            {
                AppName = "Flyingdarts",
                HostedZone = props.HostedZone,
                Certificate = props.Certificate,
                DeploymentEnvironment = props.DeploymentEnvironment,
            }
        );

        CnameRecord = new CnameRecord(
            this,
            props.GetUniqueResourceId(nameof(CnameRecord)),
            new CnameRecordProps
            {
                Zone = props.HostedZone,
                RecordName = props.DeploymentEnvironment.GetCnameRecordName(),
                DomainName = WebApp.Distribution.DomainName,
                Ttl = Duration.Seconds(300),
            }
        );

        if (props.DeploymentEnvironment.IsMarketing)
        {
            ARecord = new ARecord(
                this,
                props.GetUniqueResourceId(nameof(ARecord)),
                new ARecordProps
                {
                    Zone = props.HostedZone,
                    RecordName = props.DeploymentEnvironment.GetARecordName(),
                    Target = RecordTarget.FromAlias(new CloudFrontTarget(WebApp.Distribution)),
                    Ttl = Duration.Seconds(300),
                }
            );
        }

        new CfnOutput(
            this,
            props.GetUniqueResourceId(
                $"{nameof(CfnOutput)}-{nameof(DomainName)}-{props.DeploymentEnvironment.Name}"
            ),
            new CfnOutputProps
            {
                Value = DomainName,
                ExportName = GetCfnOutputExportName(nameof(DomainName)),
            }
        );
    }
}
