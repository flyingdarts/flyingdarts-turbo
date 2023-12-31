using System.Text.Json.Serialization;
using Flyingdarts.Backend.Tournaments.Challonge.Models;

namespace Flyingdarts.Backend.Tournaments.Challonge.Requests
{
    public class StartTournament : ChallongeRequest
    {
        /// <summary>
        /// Tournament ID (e.g. 10230) or URL (e.g. 'single_elim' for challonge.com/single_elim). 
        /// If assigned to a subdomain, URL format must be :subdomain-:tournament_url (e.g. 'test-mytourney' for test.challonge.com/mytourney)
        /// </summary>
        [JsonPropertyName("tournament")]
        public string Tournament { get; set; }

        /// <summary>
        /// 0 or 1; includes an array of associated participant records
        /// </summary>
        [JsonPropertyName("include_participants")]
        public bool IncludeParticipants { get; set; }

        /// <summary>
        /// 0 or 1; includes an array of associated match records
        /// </summary>
        [JsonPropertyName("include_matches")]
        public bool IncludeMatches { get; }
    }
}
