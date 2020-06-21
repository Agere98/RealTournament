using System.Collections.Generic;
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

namespace RealTournament.Pages
{
    [Authorize]
    public class MyTournamentsModel : PageModel
    {
        private readonly RealTournamentContext _context;
        private readonly IdentityContext _identity;
        private readonly UserManager<RealTournamentUser> _userManager;

        private static readonly object Locker = new object();

        public MyTournamentsModel(RealTournamentContext context, IdentityContext identity, UserManager<RealTournamentUser> userManager)
        {
            _context = context;
            _identity = identity;
            _userManager = userManager;
        }

        public string UserId { get; set; }
        public IList<(Game game, string opponent)> UpcomingGames { get; set; }

        [BindProperty]
        public int GameId { get; set; }

        public async Task<IActionResult> OnGetAsync(string err)
        {
            UserId = _userManager.GetUserId(User);
            var games = await _context.Game
                .Where(g => (g.FirstPlayerId == UserId || g.SecondPlayerId == UserId)
                            && (g.FirstPlayerWin == null || g.SecondPlayerWin == null))
                .Include(g => g.Tournament)
                .OrderBy(g => g.TournamentId)
                .ToListAsync();
            UpcomingGames = new List<(Game, string opponent)>();
            foreach (var game in games)
            {
                var opponentId = game.FirstPlayerId == UserId ? game.SecondPlayerId : game.FirstPlayerId;
                var opponent = await _identity.Users
                    .Where(u => u.Id == opponentId)
                    .Select(u => $"{u.FirstName} {u.LastName}")
                    .FirstOrDefaultAsync();
                UpcomingGames.Add((game, opponent));
            }

            if (err == "NotConsistent")
            {
                ViewData["errMsg"] = "Error: Declared results were not consistent";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostWinAsync()
        {
            var game = await _context.Game
                .FindAsync(GameId);
            UserId = _userManager.GetUserId(User);
            return DeclareResult(game, UserId, true) ?
                RedirectToPage() :
                RedirectToPage(new { err = "NotConsistent" });
        }

        public async Task<IActionResult> OnPostLoseAsync()
        {
            var game = await _context.Game
                .FindAsync(GameId);
            UserId = _userManager.GetUserId(User);
            return DeclareResult(game, UserId, false) ?
                RedirectToPage() :
                RedirectToPage(new { err = "NotConsistent" });
        }

        private bool DeclareResult(Game game, string userId, bool win)
        {
            bool consistent = true;
            if (game.FirstPlayerId == userId)
            {
                if (game.SecondPlayerWin == null)
                {
                    lock (Locker)
                    {
                        var check = _context.Game
                            .Where(g => g.Id == game.Id)
                            .Select(g => g.SecondPlayerWin)
                            .First();
                        if (check == null)
                        {
                            game.FirstPlayerWin = win;
                            _context.SaveChanges();
                        }
                        else
                        {
                            game.SecondPlayerWin = check;
                        }
                    }
                }
                if (game.SecondPlayerWin != null)
                {
                    if (game.SecondPlayerWin.Value != win)
                    {
                        game.FirstPlayerWin = win;
                    }
                    else
                    {
                        game.SecondPlayerWin = null;
                        consistent = false;
                    }
                }
            }
            else if (game.SecondPlayerId == userId)
            {
                if (game.FirstPlayerWin == null)
                {
                    lock (Locker)
                    {
                        var check = _context.Game
                            .Where(g => g.Id == game.Id)
                            .Select(g => g.FirstPlayerWin)
                            .First();
                        if (check == null)
                        {
                            game.SecondPlayerWin = win;
                            _context.SaveChanges();
                        }
                        else
                        {
                            game.FirstPlayerWin = check;
                        }
                    }
                }
                if (game.FirstPlayerWin != null)
                {
                    if (game.FirstPlayerWin.Value != win)
                    {
                        game.SecondPlayerWin = win;
                    }
                    else
                    {
                        game.FirstPlayerWin = null;
                        consistent = false;
                    }
                }
            }

            lock (Locker)
            {
                _context.SaveChanges();
            }

            return consistent;
        }
    }
}
