using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealTournament.Utility;

namespace RealTournament.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Discipline { get; set; }
        public string Organizer { get; set; }
        [Required]
        [InFuture(ErrorMessage = "You can't host tournaments in the past")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime Time { get; set; }
        [Required]
        [Display(Name = "Application deadline")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime ApplicationDeadline { get; set; }
        [Required]
        [Display(Name = "Max number of participants")]
        public int MaxParticipants { get; set; }

        public List<Participant> Participants { get; set; }
        public List<Game> Games { get; set; }
    }
}
