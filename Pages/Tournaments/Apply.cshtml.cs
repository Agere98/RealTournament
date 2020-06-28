using System;
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
    public class ApplyModel : PageModel
    {
        private readonly RealTournamentContext _context;
        private readonly UserManager<RealTournamentUser> _userManager;

        public ApplyModel(RealTournamentContext context, UserManager<RealTournamentUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

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

            TournamentId = id.Value;
            return Page();
        }

        public Tournament Tournament { get; set; }
        [BindProperty]
        public int TournamentId { get; set; }
        [BindProperty]
        public string LicenseNumber { get; set; }
        [BindProperty]
        public int CurrentRanking { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (!(await CanApply(TournamentId, userId)))
            {
                return RedirectToPage("./Details", new { id = TournamentId });
            }

            var participant = new Participant()
            {
                TournamentId = TournamentId,
                UserId = userId,
                LicenseNumber = LicenseNumber,
                Ranking = CurrentRanking
            };

            try
            {
                await _context.Participant.AddAsync(participant);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewData["errMsg"] = "Invalid participant data";
                return await OnGetAsync(TournamentId);
            }

            return RedirectToPage("./Details", new { id = TournamentId });
        }

        private async Task<bool> CanApply(int tournamentId, string userId)
        {
            var tournament = await _context.Tournament
                .FindAsync(tournamentId);
            var maxParticipants = await _context.Tournament
                .Where(t => t.Id == tournamentId)
                .Select(t => t.MaxParticipants)
                .FirstOrDefaultAsync();
            var participants = await _context.Participant
                .Where(p => p.TournamentId == tournamentId)
                .CountAsync();
            var alreadyApplied = await _context.Participant
                .Where(p => p.TournamentId == tournamentId && p.UserId == userId)
                .AnyAsync();
            return tournament != null &&
                   tournament.ApplicationDeadline > DateTime.Now &&
                   participants < maxParticipants &&
                   !alreadyApplied;
        }
    }
}
