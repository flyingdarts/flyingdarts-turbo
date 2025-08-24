using System.ComponentModel.DataAnnotations;

namespace Flyingdarts.CDK.Constructs;

public class AuthStackProps : BaseStackProps
{
    [Required]
    public required string Repository { get; init; }
    protected override string StackName => Constants.Stacks.Auth;
}
