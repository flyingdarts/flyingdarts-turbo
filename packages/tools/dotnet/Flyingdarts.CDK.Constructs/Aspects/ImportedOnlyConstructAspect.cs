namespace Flyingdarts.CDK.Constructs;

/// <summary>
/// Aspect that validates a construct only contains imported resources and doesn't create new AWS resources.
/// Apply this aspect to constructs that should only import existing resources.
/// </summary>
public class ImportedOnlyConstructAspect : DeputyBase, IAspect
{
    private readonly string _constructName;

    public ImportedOnlyConstructAspect(string constructName)
    {
        _constructName = constructName;
    }

    public void Visit(IConstruct node)
    {
        // Only check the specific construct we're interested in
        if (node is Construct construct && construct.Node.Id == _constructName)
        {
            ValidateImportedOnly(construct);
        }
    }

    private void ValidateImportedOnly(Construct construct)
    {
        var children = construct.Node.Children;

        foreach (var child in children)
        {
            // Check if the child is creating a new resource (not imported)
            if (IsCreatingNewResource(child))
            {
                throw new InvalidOperationException(
                    $"Construct '{_constructName}' should only contain imported resources. "
                        + $"Found resource-creating child: {child.GetType().Name} with ID: {child.Node.Id}"
                );
            }
        }
    }

    private bool IsCreatingNewResource(IConstruct child)
    {
        // List of known imported resource types (using From* methods)
        var importedResourceTypes = new[]
        {
            "Amazon.CDK.AWS.Route53.HostedZone",
            "Amazon.CDK.AWS.CertificateManager.Certificate",
            "Amazon.CDK.AWS.S3.Bucket",
            "Amazon.CDK.AWS.DynamoDB.Table",
            "Amazon.CDK.AWS.IAM.Role",
            "Amazon.CDK.AWS.Lambda.Function",
        };

        var childType = child.GetType().FullName;

        // If it's an imported resource type, it's allowed
        if (importedResourceTypes.Any(type => childType == type))
        {
            return false;
        }

        // Check if the child is creating new resources by looking at its constructor
        // This is a simplified check - in practice, you might want to be more specific
        var constructors = child.GetType().GetConstructors();
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            // If it has parameters that suggest resource creation (like props), it's likely creating resources
            if (parameters.Any(p => p.Name?.Contains("Props") == true || p.Name?.Contains("Options") == true))
            {
                return true;
            }
        }

        return false;
    }
}
