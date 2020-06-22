using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealTournament.Models
{
    public class Sponsor
    {
        public int Id { get; set; }
        [Required]
        public string LogoUrl { get; set; }
        public int TournamentId { get; set; }

        public Tournament Tournament { get; set; }
    }
}
