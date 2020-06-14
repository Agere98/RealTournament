using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTournament.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Discipline { get; set; }
        public string Organizer { get; set; }
        public DateTime Time { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public int MaxParticipants { get; set; }

        public List<Participant> Participants { get; set; }
        public List<Game> Games { get; set; }
    }
}
