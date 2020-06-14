using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RealTournament.Areas.Identity.Data;
using RealTournament.Data;
using RealTournament.Models;

namespace RealTournament.Pages.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly RealTournamentContext _context;
        private readonly IdentityContext _identity;

        public DetailsModel(RealTournamentContext context, IdentityContext identity)
        {
            _context = context;
            _identity = identity;
        }

        public Tournament Tournament { get; set; }
        public string Organizer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tournament = await _context.Tournament.FirstOrDefaultAsync(m => m.Id == id);
            if (Tournament == null)
            {
                return NotFound();
            }

            var organizer = await _identity.Users
                .Select(u => new { u.Id, Name = $"{u.FirstName} {u.LastName}" })
                .FirstOrDefaultAsync(m => m.Id == Tournament.Organizer);
            if (organizer == null)
            {
                return NotFound();
            }

            Organizer = organizer.Name;

            return Page();
        }
    }
}
