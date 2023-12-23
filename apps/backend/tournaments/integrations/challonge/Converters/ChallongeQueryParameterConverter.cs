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

    public static IEnumerable<(string Key, object Value)> GetNonNullSetProperties(object obj)
    {
        Type type = obj.GetType();
        string interfaceName = GetInterfaceName(type);

        return type.GetProperties()
            .Where(property => property.CanWrite && property.GetValue(obj) != null)
            .Select(property =>
            {
                string key = $"{interfaceName}[{property.Name}]".ToLower();
                object value = property.GetValue(obj)!;
                return (key, value);
            });
    }
}