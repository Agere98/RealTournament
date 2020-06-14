using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTournament.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int Phase { get; set; }
        public string FirstPlayerId { get; set; }
        public string SecondPlayerId { get; set; }
        public bool? FirstPlayerWin { get; set; }
        public bool? SecondPlayerWin { get; set; }

        public Tournament Tournament { get; set; }
        public Participant FirstPlayer { get; set; }
        public Participant SecondPlayer { get; set; }
    }
}
