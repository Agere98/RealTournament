using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public class EditModel : PageModel
    {
        private readonly RealTournamentContext _context;
        private readonly UserManager<RealTournamentUser> _userManager;

        public EditModel(RealTournamentContext context, UserManager<RealTournamentUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Tournament Tournament { get; set; }

        [BindProperty]
        [Display(Name = "Sponsor logo URLs")]
        public List<string> SponsorLogoUrls { get; set; }
        [BindProperty]
        public string NewLogo { get; set; }

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

            if (SponsorLogoUrls == null)
            {
                SponsorLogoUrls = await _context.Sponsor
                    .Where(s => s.TournamentId == Tournament.Id)
                    .Select(s => s.LogoUrl)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var original = await _context.Tournament.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == Tournament.Id);
            Tournament.Organizer = original.Organizer;
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var userId = _userManager.GetUserId(User);
            if (userId != Tournament.Organizer)
            {
                return Unauthorized();
            }

            _context.Attach(Tournament).State = EntityState.Modified;

            try
            {
                _context.RemoveRange(_context.Sponsor
                    .Where(s => s.TournamentId == Tournament.Id));
                foreach (var url in SponsorLogoUrls)
                {
                    await _context.Sponsor.AddAsync(new Sponsor
                    {
                        LogoUrl = url,
                        TournamentId = Tournament.Id
                    });
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(Tournament.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                ViewData["errMsg"] = "Invalid tournament data";
                return await OnGetAsync(Tournament.Id);
            }

            return RedirectToPage("./Details", new { id = Tournament.Id });
        }

        public IActionResult OnPostAddLogo()
        {
            if (!string.IsNullOrEmpty(NewLogo))
            {
                SponsorLogoUrls.Add(NewLogo);
                NewLogo = "";
            }

            return Page();
        }

        public IActionResult OnPostRemoveLogo(string remove)
        {
            if (!string.IsNullOrEmpty(NewLogo) && int.TryParse(remove, out var i))
            {
                SponsorLogoUrls.RemoveAt(i);
            }

            return Page();
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournament.Any(e => e.Id == id);
        }
    }
}
