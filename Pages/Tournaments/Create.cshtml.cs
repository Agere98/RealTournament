using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Tournament = new Tournament
            {
                Latitude = 52.40359f,
                Longitude = 16.95006f
            };
            return Page();
        }

        [BindProperty]
        public Tournament Tournament { get; set; }

        [BindProperty]
        [Display(Name = "Sponsor logo URLs")]
        public List<string> SponsorLogoUrls { get; set; } = new List<string>();
        [BindProperty]
        public string NewLogo { get; set; }

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
                foreach (var url in SponsorLogoUrls)
                {
                    await _context.Sponsor.AddAsync(new Sponsor
                    {
                        LogoUrl = url,
                        Tournament = Tournament
                    });
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewData["errMsg"] = "Invalid tournament data";
                return OnGet();
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
    }
}
