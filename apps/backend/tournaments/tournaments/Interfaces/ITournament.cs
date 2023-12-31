using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyingdarts.Backend.Tournaments.Interfaces
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
