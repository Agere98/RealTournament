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
    public class CreateModel : PageModel
    {
        private readonly RealTournamentContext _context;
        private readonly UserManager<RealTournamentUser> _userManager;

        public CreateModel(RealTournamentContext context, UserManager<RealTournamentUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Tournament Tournament { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Tournament.Organizer = _userManager.GetUserId(User);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _context.Tournament.AddAsync(Tournament);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewData["errMsg"] = "Invalid tournament data";
                return OnGet();
            }

            return RedirectToPage("./Details", new { id = Tournament.Id });
        }
    }
}
