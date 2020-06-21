using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealTournament.Data;
using RealTournament.Models;

namespace RealTournament.Services
{
    public class Matchmaker
    {
        private readonly RealTournamentContext _context;
        private readonly Tournament _tournament;

        public Matchmaker(RealTournamentContext context, Tournament tournament)
        {
            _context = context;
            _tournament = tournament;
        }

        public async Task LaunchTournament()
        {
            var participants = await _context.Participant
                .Where(p => p.TournamentId == _tournament.Id)
                .ToListAsync();
            var n = participants.Count;
            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    var game = new Game()
                    {
                        TournamentId = _tournament.Id,
                        FirstPlayerId = participants[i].UserId,
                        SecondPlayerId = participants[j].UserId
                    };
                    await _context.AddAsync(game);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
