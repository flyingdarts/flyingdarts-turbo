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

    public static readonly DeploymentEnvironment Marketing = new("Marketing", "www");
    public static readonly DeploymentEnvironment Development = new("Development", "dev");
    public static readonly DeploymentEnvironment Staging = new("Staging", "staging");
    public static readonly DeploymentEnvironment Production = new("Production", "game");
    public static readonly DeploymentEnvironment None = new("None", null);

    public string[] DomainNames
    {
        get
        {
            if (IsMarketing)
            {
                return [$"{Subdomain}.{Constants.DomainName}", Constants.DomainName];
            }

            return [$"{Subdomain}.{Constants.DomainName}"];
        }
    }

    public string GetCnameRecordName()
    {
        if (!IsValidDeploymentEnvironment)
        {
            throw new InvalidOperationException(
                "CnameRecordName is only valid for valid deployment environments"
            );
        }

        return $"{Subdomain}.{Constants.DomainName}";
    }

    public string GetARecordName()
    {
        if (!IsMarketing)
        {
            throw new InvalidOperationException(
                "ARecordName is only valid for marketing environment"
            );
        }

        return $"{Constants.DomainName}";
    }

    public bool IsProduction => this == Production;
    public bool IsStaging => this == Staging;
    public bool IsDevelopment => this == Development;
    public bool IsMarketing => this == Marketing;
    public bool IsValidDeploymentEnvironment => this != None;

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
