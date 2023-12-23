using Flyingdarts.Backend.Tournaments.Challonge.Models;
using System;
using System.Text.Json.Serialization;

namespace Flyingdarts.Backend.Tournaments.Challonge.Requests
{
    public class CreateTournament : ChallongeRequest
    {
        /// <summary>
        /// Your event's name/title (Max: 60 characters)
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Single elimination (default), double elimination, round robin, swiss
        /// </summary>
        [JsonPropertyName("tournament_type")]
        public ChallongeTournamentType TournamentType { get; set; }

        /// <summary>
        /// challonge.com/url (letters, numbers, and underscores only); when blank on create, a random URL will be generated for you
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }

        /// <summary>
        /// subdomain.challonge.com/url (Requires write access to the specified subdomain)
        /// </summary>
        [JsonPropertyName("subdomain")]
        public string Subdomain { get; set; }

        /// <summary>
        /// Description/instructions to be displayed above the bracket
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// True or false. Have Challonge host a sign-up page (otherwise, you manually add all participants)
        /// </summary>
        [JsonPropertyName("open_signup")]
        public bool OpenSignup { get; set; }

        /// <summary>
        /// True or false - Single Elimination only. Include a match between semifinal losers? (default: false)
        /// </summary>
        [JsonPropertyName("hold_third_place_match")]
        public bool HoldThirdPlaceMatch { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Swiss only - default: 1.0
        /// </summary>
        [JsonPropertyName("pts_for_match_win")]
        public decimal PointsForMatchWin { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Swiss only - default: 0.5
        /// </summary>
        [JsonPropertyName("pts_for_match_tie")]
        public decimal PointsForMatchTie { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Swiss only - default: 0.0
        /// </summary>
        [JsonPropertyName("pts_for_game_win")]
        public decimal PointsForGameWin { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Swiss only - default: 0.0
        /// </summary>
        [JsonPropertyName("pts_for_game_tie")]
        public decimal PointsForGameTie { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Swiss only - default: 1.0
        /// </summary>
        [JsonPropertyName("pts_for_bye")]
        public decimal PointsForBye { get; set; }

        /// <summary>
        /// Integer - Swiss only - We recommend limiting the number of rounds to less than two-thirds the number of players.
        /// Otherwise, an impossible pairing situation can be reached, and your tournament may end before the desired number of rounds are played.
        /// </summary>
        [JsonPropertyName("swiss_rounds")]
        public int SwissRounds { get; set; }

        /// <summary>
        /// One of the following: 'match wins', 'game wins', 'points scored', 'points difference', 'custom'
        /// </summary>
        [JsonPropertyName("ranked_by")]
        public ChallongeRankedBy RankedBy { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Round Robin "custom only" - default: 1.0
        /// </summary>
        [JsonPropertyName("rr_pts_for_match_win")]
        public decimal RoundRobinPointsForMatchWin { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Round Robin "custom" only - default: 0.5
        /// </summary>
        [JsonPropertyName("rr_pts_for_match_tie")]
        public decimal RoundRobinPointsForMatchTie { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Round Robin "custom" only - default: 0.0
        /// </summary>
        [JsonPropertyName("rr_pts_for_game_win")]
        public decimal RoundRobinPointsForGameWin { get; set; }

        /// <summary>
        /// Decimal (to the nearest tenth) - Round Robin "custom" only - default: 0.0
        /// </summary>
        [JsonPropertyName("rr_pts_for_game_tie")]
        public decimal RoundRobinPointsForGameTie { get; set; }

        /// <summary>
        /// True or false - Allow match attachment uploads (default: false)
        /// </summary>
        [JsonPropertyName("accept_attachments")]
        public bool AcceptAttachments { get; set; }

        /// <summary>
        /// True or false - Single & Double Elimination only - Label each round above the bracket (default: false)
        /// </summary>
        [JsonPropertyName("show_rounds")]
        public bool ShowRounds { get; set; }

        /// <summary>
        /// True or false - Hide this tournament from the public browsable index and your profile (default: false)
        /// </summary>
        [JsonPropertyName("private")]
        public bool Private { get; set; }

        /// <summary>
        /// True or false - Email registered Challonge participants when matches open up for them (default: false)
        /// </summary>
        [JsonPropertyName("notify_users_when_matches_open")]
        public bool NotifyUsersWhenMatchesOpen { get; set; }

        /// <summary>
        /// True or false - Email registered Challonge participants the results when this tournament ends (default: false)
        /// </summary>
        [JsonPropertyName("notify_users_when_the_tournament_ends")]
        public bool NotifyUsersWhenTheTournamentEnds { get; set; }

        /// <summary>
        /// True or false - Instead of traditional seeding rules, make pairings by going straight down the list of participants.
        /// First-round matches are filled in top to bottom, then qualifying matches (if applicable). (default: false)
        /// </summary>
        [JsonPropertyName("sequential_pairings")]
        public bool SequentialPairings { get; set; }

        /// <summary>
        /// Integer - Maximum number of participants in the bracket. A waiting list (attribute on Participant) will capture participants once the cap is reached.
        /// </summary>
        [JsonPropertyName("signup_cap")]
        public int SignupCap { get; set; }

        /// <summary>
        /// Datetime - the planned or anticipated start time for the tournament (Used with check_in_duration to determine participant check-in window). Timezone defaults to Eastern.
        /// </summary>
        [JsonPropertyName("start_at")]
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Integer - Length of the participant check-in window in minutes.
        /// </summary>
        [JsonPropertyName("check_in_duration")]
        public int CheckInDuration { get; set; }

        /// <summary>
        /// String - This option only affects double elimination. null/blank (default) - give the winners bracket finalist two chances to beat the losers bracket finalist, 'single match' - create only one grand finals match, 'skip' - don't create a finals match between winners and losers bracket finalists
        /// </summary>
        [JsonPropertyName("grand_finals_modifier")]
        public string GrandFinalsModifier { get; set; }
    }
}
