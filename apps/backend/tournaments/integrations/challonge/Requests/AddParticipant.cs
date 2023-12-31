using Flyingdarts.Backend.Tournaments.Challonge.Interfaces;
using Flyingdarts.Backend.Tournaments.Challonge.Models;

namespace Flyingdarts.Backend.Tournaments.Challonge.Requests
{
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

