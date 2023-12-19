public static class ChallongeConvertibleExtensions
{
    public static Dictionary<string, object> ToChallongeDictionary(this ChallongeConvertible convertible)
    {
        return ChallongeQueryParameterConverter.GetNonNullSetProperties(convertible);
    }
}