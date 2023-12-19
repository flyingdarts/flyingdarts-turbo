public abstract class ChallongeConvertible
{
    public Dictionary<string, object> ToChallongeDictionary()
    {
        return ChallongeQueryParameterConverter.GetNonNullSetProperties(this);
    }
}