namespace Flyingdarts.CDK.Constructs.Tests;

public class DeploymentEnvironmentEqualityTests
{
    [Fact]
    public void DeploymentEnvironment_EqualityOperators_WorkCorrectly()
    {
        // Arrange
        var development1 = DeploymentEnvironment.Development;
        var development2 = DeploymentEnvironment.Development;
        var staging = DeploymentEnvironment.Staging;
        var production = DeploymentEnvironment.Production;
        var none = DeploymentEnvironment.None;

        // Act & Assert - Equality
        Assert.True(development1 == development2);
        Assert.True(development1 == DeploymentEnvironment.Development);
        Assert.True(staging == DeploymentEnvironment.Staging);
        Assert.True(production == DeploymentEnvironment.Production);
        Assert.True(none == DeploymentEnvironment.None);

        // Act & Assert - Inequality
        Assert.True(development1 != staging);
        Assert.True(development1 != production);
        Assert.True(development1 != none);
        Assert.True(staging != production);
        Assert.True(staging != none);
        Assert.True(production != none);
    }

    [Fact]
    public void DeploymentEnvironment_EqualsMethod_WorksCorrectly()
    {
        // Arrange
        var development1 = DeploymentEnvironment.Development;
        var development2 = DeploymentEnvironment.Development;
        var staging = DeploymentEnvironment.Staging;

        // Act & Assert
        Assert.True(development1.Equals(development2));
        Assert.True(development1.Equals(DeploymentEnvironment.Development));
        Assert.False(development1.Equals(staging));
        Assert.False(development1.Equals(DeploymentEnvironment.Staging));
    }

    [Fact]
    public void DeploymentEnvironment_GetHashCode_IsConsistent()
    {
        // Arrange
        var development1 = DeploymentEnvironment.Development;
        var development2 = DeploymentEnvironment.Development;
        var staging = DeploymentEnvironment.Staging;

        // Act & Assert
        Assert.Equal(development1.GetHashCode(), development2.GetHashCode());
        Assert.NotEqual(development1.GetHashCode(), staging.GetHashCode());
    }

    [Fact]
    public void DeploymentEnvironment_CustomInstances_EqualityWorks()
    {
        // Arrange
        var custom1 = new DeploymentEnvironment("Custom", "custom");
        var custom2 = new DeploymentEnvironment("Custom", "custom");
        var custom3 = new DeploymentEnvironment("Custom", "different");
        var custom4 = new DeploymentEnvironment("Different", "custom");

        // Act & Assert
        Assert.True(custom1 == custom2);
        Assert.False(custom1 == custom3);
        Assert.False(custom1 == custom4);
        Assert.True(custom1 != custom3);
        Assert.True(custom1 != custom4);
    }

    [Fact]
    public void DeploymentEnvironment_WithNullSubdomain_EqualityWorks()
    {
        // Arrange
        var production1 = DeploymentEnvironment.Production;
        var production2 = DeploymentEnvironment.Production;
        var none1 = DeploymentEnvironment.None;
        var none2 = DeploymentEnvironment.None;
        var customNull1 = new DeploymentEnvironment("Custom", null);
        var customNull2 = new DeploymentEnvironment("Custom", null);

        // Act & Assert
        Assert.True(production1 == production2);
        Assert.True(none1 == none2);
        Assert.True(customNull1 == customNull2);
        Assert.False(production1 == none1);
        Assert.False(production1 == customNull1);
    }

    [Fact]
    public void DeploymentEnvironment_ComparisonInConditional_WorksCorrectly()
    {
        // Arrange
        var environments = new[]
        {
            DeploymentEnvironment.Development,
            DeploymentEnvironment.Staging,
            DeploymentEnvironment.Production,
            DeploymentEnvironment.None,
        };

        // Act
        var isProduction = environments.Any(env => env == DeploymentEnvironment.Production);
        var isDevelopment = environments.Any(env => env == DeploymentEnvironment.Development);
        var isStaging = environments.Any(env => env == DeploymentEnvironment.Staging);
        var isNone = environments.Any(env => env == DeploymentEnvironment.None);

        // Assert
        Assert.True(isProduction);
        Assert.True(isDevelopment);
        Assert.True(isStaging);
        Assert.True(isNone);
    }
}
