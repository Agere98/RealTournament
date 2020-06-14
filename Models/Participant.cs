using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTournament.Models
{
    public class Participant
    {
        public int TournamentId { get; set; }
        public string UserId { get; set; }
        public string LicenseNumber { get; set; }
        public int Ranking { get; set; }

        public Tournament Tournament { get; set; }
        public List<Game> GamesAsFirst { get; set; }
        public List<Game> GamesAsSecond { get; set; }
    }
}
