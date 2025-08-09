namespace Flyingdarts.CDK.Constructs;

public class WebApplicationHostingConstruct : Construct
{
    public Bucket S3Bucket { get; }
    public Distribution Distribution { get; }
    private S3OriginAccessControl OriginAccessControl { get; }

    public WebApplicationHostingConstruct(
        Construct scope,
        string id,
        WebApplicationHostingConstructProps props
    )
        : base(scope, id)
    {
        // Create S3 Bucket
        S3Bucket = CreateS3Bucket(props);

        // Add security policy to the S3 bucket
        CreateS3BucketPolicy();

        // Setup Origin Access Control (OAC)
        OriginAccessControl = CreateOriginAccessControl(props);

        // Create CloudFront distribution
        Distribution = CreateDistribution(props);
    }

    private S3OriginAccessControl CreateOriginAccessControl(
        WebApplicationHostingConstructProps props
    )
    {
        // Setup Origin Access Control (OAC)
        return new S3OriginAccessControl(
            this,
            props.GetResourceIdentifier(nameof(OriginAccessControl)),
            new S3OriginAccessControlProps
            {
                OriginAccessControlName = props
                    .GetResourceIdentifier(nameof(OriginAccessControl))
                    .Replace("-", "")
                    .ToLower(),
                Description =
                    $"Origin Access Control that allows CloudFront to access the S3 {props.DeploymentEnvironment.Name} bucket for {props.AppName} securely",
                Signing = new Signing(SigningProtocol.SIGV4, SigningBehavior.ALWAYS),
            }
        );
    }

    private Distribution CreateDistribution(WebApplicationHostingConstructProps props)
    {
        // Get existing certificate or throw if not provided
        if (props.Certificate == null)
        {
            throw new Exception("Certificate is required");
        }

        // Create the CloudFront distribution
        return new Distribution(
            this,
            props.GetResourceIdentifier(nameof(Distribution)),
            new DistributionProps
            {
                DefaultBehavior = new BehaviorOptions
                {
                    Origin = S3BucketOrigin.WithOriginAccessControl(
                        S3Bucket,
                        new S3BucketOriginWithOACProps { OriginAccessControl = OriginAccessControl }
                    ),
                    ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
                    Compress = true,
                    AllowedMethods = AllowedMethods.ALLOW_GET_HEAD_OPTIONS,
                    CachedMethods = CachedMethods.CACHE_GET_HEAD_OPTIONS,
                },
                DefaultRootObject = "index.html",
                Comment =
                    $"CloudFront distribution for serving our {Constants.DomainName} website in {props.DeploymentEnvironment.Name} environment",
                PriceClass = PriceClass.PRICE_CLASS_100,
                DomainNames = props.DeploymentEnvironment.DomainNames,
                Certificate = props.Certificate,
            }
        );
    }

    private void CreateS3BucketPolicy()
    {
        // 1. Deny non-HTTPS access
        S3Bucket.AddToResourcePolicy(
            new PolicyStatement(
                new PolicyStatementProps
                {
                    Effect = Effect.DENY,
                    Principals = new[] { new AnyPrincipal() },
                    Actions = ["s3:*"],
                    Resources = [S3Bucket.BucketArn, $"{S3Bucket.BucketArn}/*"],
                    Conditions = new Dictionary<string, object>
                    {
                        ["Bool"] = new Dictionary<string, object>
                        {
                            ["aws:SecureTransport"] = "false",
                        },
                    },
                }
            )
        );
    }

    private Bucket CreateS3Bucket(WebApplicationHostingConstructProps props)
    {
        // Create the S3 bucket with exact specifications
        return new Bucket(
            this,
            props.GetResourceIdentifier(nameof(Bucket)),
            new BucketProps
            {
                BucketName = props.GetResourceIdentifier(nameof(Bucket)).Replace("-", "").ToLower(),
                Versioned = false, // Versioning disabled as per specs
                RemovalPolicy = RemovalPolicy.DESTROY,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL, // Block all public access
                Encryption = BucketEncryption.S3_MANAGED, // SSE-S3 encryption
                EnforceSSL = true, // Require HTTPS
                ObjectOwnership = ObjectOwnership.BUCKET_OWNER_ENFORCED, // ACLs disabled
                Cors =
                [
                    new CorsRule
                    {
                        AllowedMethods = [HttpMethods.GET, HttpMethods.HEAD],
                        AllowedOrigins = ["*"],
                        AllowedHeaders = ["*"],
                        MaxAge = 3000,
                    },
                ],
            }
        );
    }
}
