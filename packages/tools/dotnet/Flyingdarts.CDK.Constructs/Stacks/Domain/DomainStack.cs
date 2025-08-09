namespace Flyingdarts.CDK.Constructs;

/// <summary>
/// This stack is environment agnostic.
/// This stack contains domain-related resources (HostedZone and Certificate) that are imported from existing AWS resources.
/// This stack is validated to only contain imported resources, not create new ones.
/// </summary>
public class DomainStack : Stack
{
    public IHostedZone FlyingdartsHostedZone { get; }
    public ICertificate FlyingdartsCertificate { get; }

    public DomainStack(Construct scope, DomainStackProps props)
        : base(scope, props.StackId, new StackProps { Env = props.StackEnvironment })
    {
        var (hostedZoneId, certificateArn) = (
            System.Environment.GetEnvironmentVariable("FLYINGDARTS_AWS_HOSTED_ZONE"),
            System.Environment.GetEnvironmentVariable("FLYINGDARTS_AWS_CERTIFICATE")
        );

        if (string.IsNullOrEmpty(hostedZoneId) || string.IsNullOrEmpty(certificateArn))
        {
            throw new Exception(
                "FLYINGDARTS_AWS_HOSTED_ZONE and FLYINGDARTS_AWS_CERTIFICATE must be set"
            );
        }

        FlyingdartsHostedZone = HostedZone.FromHostedZoneAttributes(
            this,
            props.GetUniqueResourceId(nameof(HostedZone)),
            new HostedZoneAttributes
            {
                HostedZoneId = hostedZoneId,
                ZoneName = Constants.DomainName,
            }
        );

        FlyingdartsCertificate = Certificate.FromCertificateArn(
            this,
            $"{Constants.Stacks.Domain}-{nameof(Certificate)}",
            certificateArn
        );

        // Apply aspect to ensure this stack only contains imported resources
        AmazonAspect.Of(this).Add(new ImportedOnlyConstructAspect(props.StackId));
    }
}
