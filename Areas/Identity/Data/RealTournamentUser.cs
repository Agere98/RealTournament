using Microsoft.AspNetCore.Identity;

namespace RealTournament.Areas.Identity.Data
{
    public class RealTournamentUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
    }
}
