using System.Text.Json.Serialization;

namespace Flyingdarts.Backend.Tournaments
{
    public class CreateParticipant
    {
        public string Name { get; set; }

        public Dictionary<string, object> ToChallongeMap()
        {
            return new Dictionary<string, object> { { "participant[name]", Name } };
        }
    }
}