using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTournament.Areas.Identity.Data;
using RealTournament.Data;
using RealTournament.Models;

namespace RealTournament.ViewComponents
{
    public class ScoreboardViewComponent : ViewComponent
    {
        public class PlayerResultsModel
        {
            public int Rank { get; set; }
            public string PlayerName { get; set; }
            public int Score { get; set; }
            public int GamesPlayed { get; set; }
            public List<(string Opponent, bool Win)> GameResults { get; set; }
        }

        private readonly RealTournamentContext _context;
        private readonly IdentityContext _identity;

        private readonly Dictionary<string, string> participantNames;

        public ScoreboardViewComponent(RealTournamentContext context, IdentityContext identity)
        {
            _context = context;
            _identity = identity;
            participantNames = new Dictionary<string, string>();
        }

        public async Task<IViewComponentResult> InvokeAsync(Tournament tournament)
        {
            var participants = await _context.Participant
                .Where(p => p.TournamentId == tournament.Id)
                .Include(p => p.GamesAsFirst)
                .Include(p => p.GamesAsSecond)
                .ToListAsync();
            var scoreboard = new List<PlayerResultsModel>();
            foreach (var participant in participants)
            {
                var results = new PlayerResultsModel
                {
                    PlayerName = await GetParticipantName(participant.UserId),
                    GameResults = new List<(string Opponent, bool Win)>(),
                    Rank = participant.Ranking
                };
                foreach (var game in participant.GamesAsFirst)
                {
                    if (game.FirstPlayerWin == null || !game.FirstPlayerWin != game.SecondPlayerWin) continue;
                    results.GameResults.Add((await GetParticipantName(game.SecondPlayerId), game.FirstPlayerWin.Value));
                    results.GamesPlayed++;
                    if (game.FirstPlayerWin.Value) results.Score++;
                }
                foreach (var game in participant.GamesAsSecond)
                {
                    if (game.FirstPlayerWin == null || !game.FirstPlayerWin != game.SecondPlayerWin) continue;
                    results.GameResults.Add((await GetParticipantName(game.FirstPlayerId), game.SecondPlayerWin.Value));
                    results.GamesPlayed++;
                    if (game.SecondPlayerWin.Value) results.Score++;
                }
                scoreboard.Add(results);
            }
            scoreboard.Sort((first, second) =>
                first.Score > second.Score ? -1 :
                    first.Score < second.Score ? 1 :
                        first.Rank.CompareTo(second.Rank)
            );
            for (var i = 0; i < scoreboard.Count; i++)
            {
                scoreboard[i].Rank = i + 1;
            }
            return View(scoreboard);
        }

        public async Task<string> GetParticipantName(string userId)
        {
            if (participantNames.TryGetValue(userId, out var name))
            {
                return name;
            }

            name = (await _identity.Users
                       .Where(u => u.Id == userId)
                       .Select(u => $"{u.FirstName} {u.LastName}")
                       .FirstOrDefaultAsync()) ?? "?";
            participantNames.TryAdd(userId, name);
            return name;
        }
    }
}
