internal class ChallongeQueryParameterConverter
{
    static string GetInterfaceName(Type type)
    {
        string interfaceName = type.GetInterfaces()
            .Where(i => i.Name.StartsWith("I"))
            .Select(i => i.Name.Substring(1))
            .FirstOrDefault() ?? type.Name;

        return interfaceName;
    }

    public static Dictionary<string, object> GetNonNullSetProperties(object obj)
    {
        Dictionary<string, object> nonNullProperties = new Dictionary<string, object>();

        Type type = obj.GetType();
        string interfaceName = GetInterfaceName(type);

        foreach (var property in type.GetProperties())
        {
            // Check if the property has a public setter and the value is not null
            if (property.CanWrite && property.GetValue(obj) != null)
            {
                // Interpolate interfaceName with propertyName to form the key
                string key = $"{interfaceName}[{property.Name}]".ToLower();
                nonNullProperties.Add(key, property.GetValue(obj)!);
            }
        }

        return nonNullProperties;
    }
}