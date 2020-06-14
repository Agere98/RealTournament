using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RealTournament.Areas.Identity.Data;
using RealTournament.Data;
using RealTournament.Models;

namespace RealTournament.Pages.Tournaments
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly RealTournamentContext _context;
        private readonly UserManager<RealTournamentUser> _userManager;

        public DeleteModel(RealTournamentContext context, UserManager<RealTournamentUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Tournament Tournament { get; set; }

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

            var userId = _userManager.GetUserId(User);
            if (userId != Tournament.Organizer)
            {
                return Unauthorized();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tournament = await _context.Tournament.FindAsync(id);

            if (Tournament != null)
            {
                var userId = _userManager.GetUserId(User);
                if (userId != Tournament.Organizer)
                {
                    return Unauthorized();
                }
                _context.Tournament.Remove(Tournament);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Index");
        }
    }
}
