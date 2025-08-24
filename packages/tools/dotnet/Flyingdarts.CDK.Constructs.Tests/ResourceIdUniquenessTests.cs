namespace Flyingdarts.CDK.Constructs.Tests;

public class ResourceIdUniquenessTests
{
    // Test implementation of BaseStackProps for testing
    private class TestStackProps : BaseStackProps
    {
        protected override string StackName => "TestStack";
    }

    // Test implementation of BaseConstructProps for testing
    private class TestConstructProps : BaseConstructProps
    {
        protected override string ConstructName => "TestConstruct";
    }

    [Fact]
    public void BaseStackProps_GetUniqueResourceId_WithDifferentEnvironments_ReturnsUniqueValues()
    {
        // Arrange
        var developmentProps = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        var stagingProps = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Staging,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        var productionProps = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Production,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        var resourceName = "TestResource";

        // Act
        var developmentId = developmentProps.GetUniqueResourceId(resourceName);
        var stagingId = stagingProps.GetUniqueResourceId(resourceName);
        var productionId = productionProps.GetUniqueResourceId(resourceName);

        // Assert
        Assert.Equal("TestStack-TestResource-Development", developmentId);
        Assert.Equal("TestStack-TestResource-Staging", stagingId);
        Assert.Equal("TestStack-TestResource-Production", productionId);

        // Verify uniqueness
        var allIds = new HashSet<string> { developmentId, stagingId, productionId };
        Assert.Equal(3, allIds.Count);
    }

    [Fact]
    public void BaseStackProps_GetUniqueResourceId_WithDifferentResources_ReturnsUniqueValues()
    {
        // Arrange
        var props = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        var resource1 = "Resource1";
        var resource2 = "Resource2";
        var resource3 = "Resource3";

        // Act
        var id1 = props.GetUniqueResourceId(resource1);
        var id2 = props.GetUniqueResourceId(resource2);
        var id3 = props.GetUniqueResourceId(resource3);

        // Assert
        Assert.Equal("TestStack-Resource1-Development", id1);
        Assert.Equal("TestStack-Resource2-Development", id2);
        Assert.Equal("TestStack-Resource3-Development", id3);

        // Verify uniqueness
        var allIds = new HashSet<string> { id1, id2, id3 };
        Assert.Equal(3, allIds.Count);
    }

    [Fact]
    public void BaseConstructProps_GetResourceIdentifier_WithDifferentEnvironments_ReturnsUniqueValues()
    {
        // Arrange
        var developmentProps = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
        };

        var stagingProps = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Staging,
        };

        var productionProps = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Production,
        };

        var resource = "TestResource";

        // Act
        var developmentId = developmentProps.GetResourceIdentifier(resource);
        var stagingId = stagingProps.GetResourceIdentifier(resource);
        var productionId = productionProps.GetResourceIdentifier(resource);

        // Assert
        Assert.Equal("TestConstruct-TestResource-Development", developmentId);
        Assert.Equal("TestConstruct-TestResource-Staging", stagingId);
        Assert.Equal("TestConstruct-TestResource-Production", productionId);

        // Verify uniqueness
        var allIds = new HashSet<string> { developmentId, stagingId, productionId };
        Assert.Equal(3, allIds.Count);
    }

    [Fact]
    public void BaseConstructProps_GetResourceIdentifier_WithDifferentResources_ReturnsUniqueValues()
    {
        // Arrange
        var props = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
        };

        var resource1 = "Resource1";
        var resource2 = "Resource2";
        var resource3 = "Resource3";

        // Act
        var id1 = props.GetResourceIdentifier(resource1);
        var id2 = props.GetResourceIdentifier(resource2);
        var id3 = props.GetResourceIdentifier(resource3);

        // Assert
        Assert.Equal("TestConstruct-Resource1-Development", id1);
        Assert.Equal("TestConstruct-Resource2-Development", id2);
        Assert.Equal("TestConstruct-Resource3-Development", id3);

        // Verify uniqueness
        var allIds = new HashSet<string> { id1, id2, id3 };
        Assert.Equal(3, allIds.Count);
    }

    [Fact]
    public void BaseConstructProps_ConstructId_WithDifferentEnvironments_ReturnsUniqueValues()
    {
        // Arrange
        var developmentProps = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
        };

        var stagingProps = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Staging,
        };

        var productionProps = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Production,
        };

        // Act
        var developmentId = developmentProps.ConstructId;
        var stagingId = stagingProps.ConstructId;
        var productionId = productionProps.ConstructId;

        // Assert
        Assert.Equal("TestConstruct-Construct-Development", developmentId);
        Assert.Equal("TestConstruct-Construct-Staging", stagingId);
        Assert.Equal("TestConstruct-Construct-Production", productionId);

        // Verify uniqueness
        var allIds = new HashSet<string> { developmentId, stagingId, productionId };
        Assert.Equal(3, allIds.Count);
    }

    [Fact]
    public void BaseStackProps_StackId_WithDifferentEnvironments_ReturnsUniqueValues()
    {
        // Arrange
        var developmentProps = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        var stagingProps = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Staging,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        var productionProps = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Production,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        // Act
        var developmentId = developmentProps.StackId;
        var stagingId = stagingProps.StackId;
        var productionId = productionProps.StackId;

        // Assert
        Assert.Equal("TestStack-Development", developmentId);
        Assert.Equal("TestStack-Staging", stagingId);
        Assert.Equal("TestStack-Production", productionId);

        // Verify uniqueness
        var allIds = new HashSet<string> { developmentId, stagingId, productionId };
        Assert.Equal(3, allIds.Count);
    }

    [Fact]
    public void BaseStackProps_StackId_WithNoneEnvironment_ReturnsStackNameOnly()
    {
        // Arrange
        var props = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.None,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        // Act
        var stackId = props.StackId;

        // Assert
        Assert.Equal("TestStack", stackId);
    }

    [Fact]
    public void CrossClass_ResourceIds_WithSameEnvironmentAndResource_AreDifferent()
    {
        // Arrange
        var stackProps = new TestStackProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
            StackEnvironment = new Amazon.CDK.Environment
            {
                Account = "000000000000",
                Region = "us-east-1",
            },
        };

        var constructProps = new TestConstructProps
        {
            DeploymentEnvironment = DeploymentEnvironment.Development,
        };

        var resource = "TestResource";

        // Act
        var stackResourceId = stackProps.GetUniqueResourceId(resource);
        var constructResourceId = constructProps.GetResourceIdentifier(resource);

        // Assert
        Assert.Equal("TestStack-TestResource-Development", stackResourceId);
        Assert.Equal("TestConstruct-TestResource-Development", constructResourceId);
        Assert.NotEqual(stackResourceId, constructResourceId);
    }
}
