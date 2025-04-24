using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Companies
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private const int PageSize = 6;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Company> Companies { get; set; } = new List<Company>();

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Companies
                .Include(c => c.Vacancies)
                .AsQueryable();

            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(c => 
                    c.Name.Contains(Search) || 
                    c.Description.Contains(Search));
            }

            var count = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            Companies = await query
                .OrderByDescending(c => c.Vacancies.Count)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}