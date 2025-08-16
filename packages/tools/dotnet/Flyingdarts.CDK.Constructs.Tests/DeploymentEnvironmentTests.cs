namespace Flyingdarts.CDK.Constructs.Tests;

public class DeploymentEnvironmentTests
{
    [Fact]
    public void AllEnvironments_HaveCorrectDomainNames()
    {
        // Arrange
        var dev = DeploymentEnvironment.Development.DomainNames;
        var staging = DeploymentEnvironment.Staging.DomainNames;
        var production = DeploymentEnvironment.Production.DomainNames;
        var marketing = DeploymentEnvironment.Marketing.DomainNames;

        // Assert
        Assert.Equal(dev, new[] { "dev.flyingdarts.net" });
        Assert.Equal(staging, new[] { "staging.flyingdarts.net" });
        Assert.Equal(production, new[] { "game.flyingdarts.net" });
        Assert.Equal(marketing, new[] { "www.flyingdarts.net", "flyingdarts.net" });
    }

    [Fact]
    public void OnlyMarketing_HasWWWDomainNameAndARecord()
    {
        // Arrange
        var marketing = DeploymentEnvironment.Marketing;

        // Act
        var result = marketing.GetARecordName();

        // Assert
        Assert.Equal("flyingdarts.net", result);

        // Arrange
        var otherEnvironments = new[]
        {
            DeploymentEnvironment.Development,
            DeploymentEnvironment.Staging,
            DeploymentEnvironment.Production,
        };

        // Assert throws
        foreach (var environment in otherEnvironments)
        {
            Assert.Throws<InvalidOperationException>(() => environment.GetARecordName());
        }
    }

    [Fact]
    public void GetCnameRecordName_DevelopmentEnvironment_ReturnsExpectedValue()
    {
        // Arrange
        var environment = DeploymentEnvironment.Development;

        // Act
        var result = environment.GetCnameRecordName();

        // Assert
        Assert.Equal("dev.flyingdarts.net", result);
    }

    [Fact]
    public void GetCnameRecordName_StagingEnvironment_ReturnsExpectedValue()
    {
        // Arrange
        var environment = DeploymentEnvironment.Staging;

        // Act
        var result = environment.GetCnameRecordName();

        // Assert
        Assert.Equal("staging.flyingdarts.net", result);
    }

    [Fact]
    public void GetCnameRecordName_ProductionEnvironment_ReturnsExpectedValue()
    {
        // Arrange
        var environment = DeploymentEnvironment.Production;

        // Act
        var result = environment.GetCnameRecordName();

        // Assert
        Assert.Equal("game.flyingdarts.net", result);
    }

    [Fact]
    public void GetCnameRecordName_MarketingEnvironment_ReturnsExpectedValue()
    {
        // Arrange
        var environment = DeploymentEnvironment.Marketing;

        // Act
        var result = environment.GetCnameRecordName();

        // Assert
        Assert.Equal("www.flyingdarts.net", result);
    }

    [Fact]
    public void GetCnameRecordName_NoneEnvironment_ExpectThrows()
    {
        // Arrange
        var environment = DeploymentEnvironment.None;

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            environment.GetCnameRecordName()
        );

        // Assert
        Assert.Equal(
            "CnameRecordName is only valid for valid deployment environments",
            exception.Message
        );
    }
}
