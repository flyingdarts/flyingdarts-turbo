using System.Text.Json.Serialization;

namespace Flyingdarts.Backend.Tournaments.Challonge.Models
{
    public class ChallongeRequest : ChallongeConvertible
    {
        /// <summary>
        /// Your API key (required unless you're using HTTP basic authentication)
        /// </summary>
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; }
    }
}
