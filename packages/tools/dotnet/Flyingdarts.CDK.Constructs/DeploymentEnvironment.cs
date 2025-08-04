namespace Flyingdarts.CDK.Constructs;

/// <summary>
///  The deploy environment is used to identify a Stack object  of the stack.
/// </summary>
public readonly struct DeploymentEnvironment
{
    public string Name { get; }
    public string? Subdomain { get; }

    public DeploymentEnvironment(string name, string? subdomain)
    {
        Name = name;
        Subdomain = subdomain;
    }

    public static readonly DeploymentEnvironment Development = new("Development", "dev");
    public static readonly DeploymentEnvironment Staging = new("Staging", "staging");
    public static readonly DeploymentEnvironment Production = new("Production", "www");
    public static readonly DeploymentEnvironment None = new("None", null);

    public string GetCnameRecordName()
    {
        if (!IsProduction && !IsStaging && !IsDevelopment)
        {
            throw new InvalidOperationException(
                "CnameRecordName is only valid for Production, Staging, or Development environment"
            );
        }

        return $"{Subdomain}.{Constants.DomainName}";
    }

    public string GetARecordName()
    {
        if (!IsProduction)
        {
            throw new InvalidOperationException(
                "ARecordName is only valid for Production environment"
            );
        }

        return $"{Constants.DomainName}";
    }

    public bool IsProduction => this == Production;
    public bool IsStaging => this == Staging;
    public bool IsDevelopment => this == Development;

    public static bool operator ==(DeploymentEnvironment left, DeploymentEnvironment right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DeploymentEnvironment left, DeploymentEnvironment right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        return obj is DeploymentEnvironment other && Equals(other);
    }

    public bool Equals(DeploymentEnvironment other)
    {
        return Name == other.Name && Subdomain == other.Subdomain;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Subdomain);
    }
}
