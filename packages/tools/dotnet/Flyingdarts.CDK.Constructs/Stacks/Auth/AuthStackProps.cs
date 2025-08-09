using System.ComponentModel.DataAnnotations;

namespace Flyingdarts.CDK.Constructs;

public class AuthStackProps : BaseStackProps
{
    [Required]
    public string Repository { get; set; }
    protected override string StackName => Constants.Stacks.Auth;
}
