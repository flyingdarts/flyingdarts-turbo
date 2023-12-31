using System.Text.Json.Serialization;
using Flyingdarts.Backend.Tournaments.Challonge.Models;

namespace Flyingdarts.Backend.Tournaments.Challonge.Requests
{
    public class UpdateMatch : ChallongeRequest
    {
        /// <summary>
        /// Comma separated set/game scores with player 1 score first (e.g. "1-3,3-0,3-2")
        /// </summary>
        [JsonPropertyName("scores_csv")]
        public string ScoresCSV { get; set; }

        /// <summary>
        ///	The participant ID of the winner or "tie" if applicable (Round Robin and Swiss). 
        /// NOTE: If you change the outcome of a completed match, all matches in the bracket that branch from the updated match will be reset.
        /// </summary>
        [JsonPropertyName("winner_id")]
        public string WinnerId { get; set; }

        /// <summary>
        /// Overwrites the number of votes for player 1
        /// </summary>
        [JsonPropertyName("player1_votes")]
        public string Player1Votes { get; set; }

        /// <summary>
        /// Overwrites the number of votes for player 1
        /// </summary>
        [JsonPropertyName("player2_votes")]
        public string Player2Votes { get; set; }
    }
}
