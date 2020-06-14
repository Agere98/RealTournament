using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int PageSize = 10;

        public IndexModel(RealTournamentContext context)
        {
            _context = context;
        }

        public IList<Tournament> Tournament { get; set; }
        public string SearchFilter { get; set; } = "";
        public int PageIndex { get; set; } = 1;
        public int PagesTotal { get; private set; }

        public async Task OnGetAsync(string searchFilter, int? pageIndex)
        {
            if (pageIndex.HasValue) PageIndex = pageIndex.Value;
            SearchFilter = searchFilter ?? "";
            var filtered = _context.Tournament
                .Where(t => t.Name.ToLower().Contains(SearchFilter.ToLower())
                    && t.Time > DateTime.Now)
                .OrderBy(t => t.Time);
            PagesTotal = (int)Math.Ceiling((await filtered.CountAsync()) / (double)PageSize);
            if (PageIndex > PagesTotal && PagesTotal > 0) PageIndex = PagesTotal;
            Tournament = await filtered
                .Skip(PageSize * (PageIndex - 1))
                .Take(PageSize).ToListAsync();
        }
    }
}
