using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RealTournament.Data;
using RealTournament.Models;

namespace RealTournament.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RealTournamentContext _context;

        public IndexModel(RealTournamentContext context)
        {
            _context = context;
        }

        public IList<Tournament> Tournament { get; set; }

        public async Task OnGetAsync()
        {
            Tournament = await _context.Tournament.ToListAsync();
        }
    }
}
