namespace Flyingdarts.Backend.Shared.Extensions;
public static class FluentValidationExtensions
{
    public static IRuleBuilder<T, string> IsValidGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(guid =>
        {
            Guid result;
            return Guid.TryParse(guid, out result);
        }).WithMessage("'{PropertyName}' must be a valid GUID.");
    }
}