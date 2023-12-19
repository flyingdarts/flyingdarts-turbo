// See https://aka.ms/new-console-template for more information

using SomeNamespace;

AddParticipant participant = new AddParticipant
{
    Name = "John Doe",
    Email = "john@example.com",
    Seed = "1",
    Misc = null
};
Dictionary<string, object> propertiesDictionary = participant.ToChallongeDictionary();

foreach (var kvp in propertiesDictionary)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}

namespace SomeNamespace
{

    public class ChallongeAttribute : Attribute
    {
        public string Description { get; }

        public ChallongeAttribute(string description)
        {
            Description = description;
        }
    }

    public interface IChallongeConvertible
    {
        Dictionary<string, object> ToChallongeDictionary();
    }
    public abstract class ChallongeConvertible
    {
        public Dictionary<string, object> ToChallongeDictionary()
        {
            return ChallongeQueryParameterConverter.GetNonNullSetProperties(this);
        }
    }
    public static class ChallongeConvertibleExtensions
    {
        public static Dictionary<string, object> ToChallongeDictionary(this ChallongeConvertible convertible)
        {
            return ChallongeQueryParameterConverter.GetNonNullSetProperties(convertible);
        }
    }

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
    public class ChallongeRequest : ChallongeConvertible
    {
        /// <summary>
        /// Your API key (required unless you're using HTTP basic authentication)
        /// </summary>
        public string ApiKey { get; set; }
    }
    public interface IParticipant
    {
    }
    public class AddParticipant : ChallongeRequest, IParticipant
    {
        /// <summary>
        /// The name displayed in the bracket/schedule - not required if email or challonge_username is provided. 
        /// Must be unique per tournament.
        /// </summary>
        [Challonge("name")]
        public string Name { get; set; }

        /// <summary>
        /// Provide this if the participant has a Challonge account. He or she will be invited to the tournament.
        /// </summary>
        [Challonge("challonge_username")]
        public string ChallongUsername { get; set; }

        /// <summary>
        /// Providing this will first search for a matching Challonge account. 
        /// If one is found, this will have the same effect as the "challonge_username" attribute. 
        /// If one is not found, the "new-user-email" attribute will be set, and the user will be invited via email to create an account.
        /// </summary>
        [Challonge("email")]
        public string Email { get; set; }

        /// <summary>
        /// The participant's new seed. Must be between 1 and the current number of participants (including the new record).
        /// Overwriting an existing seed will automatically bump other participants as you would expect.
        /// </summary>
        [Challonge("seed")]
        public string Seed { get; set; }

        /// <summary>
        /// Max: 255 characters. Multi-purpose field that is only visible via the API and handy for site integration (e.g. key to your users table)
        /// </summary>
        [Challonge("misc")]
        public string Misc { get; set; }
    }

}