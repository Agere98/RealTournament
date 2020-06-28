using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RealTournament.Areas.Identity.Data;
using RealTournament.Data;
using RealTournament.Models;
using RealTournament.Services;

namespace RealTournament.Pages.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly RealTournamentContext _context;
        private readonly IdentityContext _identity;
        private readonly UserManager<RealTournamentUser> _userManager;

        private static readonly object Locker = new object();

        public DetailsModel(RealTournamentContext context, IdentityContext identity, UserManager<RealTournamentUser> userManager)
        {
            _context = context;
            _identity = identity;
            _userManager = userManager;
        }

        public Tournament Tournament { get; set; }
        public string Organizer { get; set; }
        public List<string> SponsorLogoUrls { get; set; }
        public int Participants { get; set; }
        public bool CanApply { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tournament = await _context.Tournament
                .FirstOrDefaultAsync(m => m.Id == id);
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

            SponsorLogoUrls = await _context.Sponsor
                .Where(s => s.TournamentId == Tournament.Id)
                .Select(s => s.LogoUrl)
                .ToListAsync();

            Participants = await _context.Participant
                .Where(p => p.TournamentId == Tournament.Id)
                .CountAsync();

            CanApply = !(await _context.Participant
                .Where(p => p.TournamentId == id && p.UserId == _userManager.GetUserId(User))
                .AnyAsync());

            if (Participants >= Tournament.MaxParticipants || Tournament.ApplicationDeadline <= DateTime.Now)
            {
                CanApply = false;
            }

            if (Tournament.Time <= DateTime.Now && !Tournament.Ongoing)
            {
                Task matchmaking = null;
                lock (Locker)
                {
                    var check = _context.Tournament
                        .Where(t => t.Id == Tournament.Id)
                        .Select(t => t.Ongoing)
                        .First();
                    if (!check)
                    {
                        Tournament.Ongoing = true;
                        _context.SaveChanges();
                        var matchmaker = new Matchmaker(_context, Tournament);
                        matchmaking = matchmaker.LaunchTournament();
                    }
                }

                if (matchmaking != null) await matchmaking;
            }

            return Page();
        }
    }
}
