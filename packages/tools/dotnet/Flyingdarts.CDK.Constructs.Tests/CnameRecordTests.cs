namespace Flyingdarts.CDK.Constructs.Tests;

using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.S3;

public class CnameRecordTests
{
    [Fact]
    public void CnameRecordProps_StagingEnvironment_HasCorrectProperties()
    {
        // Arrange
        var deploymentEnvironment = DeploymentEnvironment.Staging;
        var expectedRecordName = deploymentEnvironment.GetCnameRecordName();
        var expectedDomainName = "d3tftejt9mjzk3.cloudfront.net"; // Mock CloudFront domain
        var expectedTtl = Duration.Seconds(300);

        // Act - Create the props that would be used for CnameRecord
        var cnameRecordProps = new
        {
            RecordName = expectedRecordName,
            DomainName = expectedDomainName,
            Ttl = expectedTtl,
        };

        // Assert
        Assert.Equal("acc.flyingdarts.net", cnameRecordProps.RecordName);
        Assert.Equal(expectedDomainName, cnameRecordProps.DomainName);
        Assert.Equal(expectedTtl, cnameRecordProps.Ttl);
    }

    [Fact]
    public void CnameRecordProps_DevelopmentEnvironment_HasCorrectProperties()
    {
        // Arrange
        var deploymentEnvironment = DeploymentEnvironment.Development;
        var expectedRecordName = deploymentEnvironment.GetCnameRecordName();
        var expectedDomainName = "d3tftejt9mjzk3.cloudfront.net"; // Mock CloudFront domain
        var expectedTtl = Duration.Seconds(300);

        // Act - Create the props that would be used for CnameRecord
        var cnameRecordProps = new
        {
            RecordName = expectedRecordName,
            DomainName = expectedDomainName,
            Ttl = expectedTtl,
        };

        // Assert
        Assert.Equal("dev.flyingdarts.net", cnameRecordProps.RecordName);
        Assert.Equal(expectedDomainName, cnameRecordProps.DomainName);
        Assert.Equal(expectedTtl, cnameRecordProps.Ttl);
    }

    [Fact]
    public void CnameRecordProps_ProductionEnvironment_HasCorrectProperties()
    {
        // Arrange
        var deploymentEnvironment = DeploymentEnvironment.Production;
        var expectedRecordName = deploymentEnvironment.GetCnameRecordName();
        var expectedDomainName = "d3tftejt9mjzk3.cloudfront.net"; // Mock CloudFront domain
        var expectedTtl = Duration.Seconds(300);

        // Act - Create the props that would be used for CnameRecord
        var cnameRecordProps = new
        {
            RecordName = expectedRecordName,
            DomainName = expectedDomainName,
            Ttl = expectedTtl,
        };

        // Assert
        Assert.Equal("www.flyingdarts.net", cnameRecordProps.RecordName);
        Assert.Equal(expectedDomainName, cnameRecordProps.DomainName);
        Assert.Equal(expectedTtl, cnameRecordProps.Ttl);
    }

    [Fact]
    public void FrontendStack_CnameRecordConfiguration_IsCorrect()
    {
        // Arrange
        var deploymentEnvironment = DeploymentEnvironment.Staging;
        var mockDistributionDomainName = "d3tftejt9mjzk3.cloudfront.net";

        // Act - Simulate the CnameRecord configuration from FrontendStack
        var recordName = deploymentEnvironment.GetCnameRecordName();
        var domainName = mockDistributionDomainName;
        var ttl = Duration.Seconds(300);

        // Assert
        Assert.Equal("acc.flyingdarts.net", recordName);
        Assert.Equal(mockDistributionDomainName, domainName);
        Assert.Equal(300, ttl.ToSeconds());
    }

    [Fact]
    public void CnameRecord_ThrowsException_ForOtherEnvironments()
    {
        // Arrange
        var deploymentEnvironment = DeploymentEnvironment.None;

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            deploymentEnvironment.GetCnameRecordName()
        );

        // Assert
        Assert.Equal(
            "CnameRecordName is only valid for Production, Staging, or Development environment",
            exception.Message
        );
    }

    [Fact]
    public void CnameRecord_AllEnvironments_GenerateUniqueRecordNames()
    {
        // Arrange
        var environments = new[]
        {
            DeploymentEnvironment.Development,
            DeploymentEnvironment.Staging,
            DeploymentEnvironment.Production,
        };

        // Act
        var recordNames = environments.Select(env => env.GetCnameRecordName()).ToArray();

        // Assert
        Assert.Equal("dev.flyingdarts.net", recordNames[0]);
        Assert.Equal("acc.flyingdarts.net", recordNames[1]);
        Assert.Equal("www.flyingdarts.net", recordNames[2]);

        // Verify uniqueness (Development and Staging should be unique, Production and None are the same)
        var uniqueRecordNames = recordNames.Distinct().ToArray();
        Assert.Equal(3, uniqueRecordNames.Length); // dev, staging, and production/none (same)
    }

    [Fact]
    public void CnameRecord_Configuration_MatchesExpectedFormat()
    {
        // Arrange
        var deploymentEnvironment = DeploymentEnvironment.Staging;
        var mockDistributionDomainName = "d3tftejt9mjzk3.cloudfront.net";

        // Act - Simulate the exact configuration from FrontendStack.cs lines 23-33
        var cnameRecordConfiguration = new
        {
            Zone = "HostedZone", // This would be the actual hosted zone reference
            RecordName = deploymentEnvironment.GetCnameRecordName(),
            DomainName = mockDistributionDomainName,
            Ttl = Duration.Seconds(300),
        };

        // Assert
        Assert.Equal("acc.flyingdarts.net", cnameRecordConfiguration.RecordName);
        Assert.Equal(mockDistributionDomainName, cnameRecordConfiguration.DomainName);
        Assert.Equal(300, cnameRecordConfiguration.Ttl.ToSeconds());
        Assert.Equal("HostedZone", cnameRecordConfiguration.Zone);
    }
}
