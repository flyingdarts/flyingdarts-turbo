using System;

public abstract class ChallongeConvertible
{
    public Dictionary<string, object> ToChallongeDictionary()
    {
        return ChallongeQueryParameterConverter.GetNonNullSetProperties(this)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}