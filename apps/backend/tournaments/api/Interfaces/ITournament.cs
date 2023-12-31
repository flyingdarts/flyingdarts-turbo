namespace Flyingdarts.Backend.Tournaments.Api.Interfaces
{
    public interface ITournament
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public enum TournamentType
    {
        SingleElimination,
        DoubleElimination,
        RoundRobin,
        Swiss
    }
}
