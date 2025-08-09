namespace Flyingdarts.CDK.Constructs.Tests;

public class DeploymentEnvironmentTests
{
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
        Assert.Equal("acc.flyingdarts.net", result);
    }

    [Fact]
    public void GetCnameRecordName_ProductionEnvironment_ReturnsExpectedValue()
    {
        // Arrange
        var environment = DeploymentEnvironment.Production;

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
            "CnameRecordName is only valid for Production, Staging, or Development environment",
            exception.Message
        );
    }
}
