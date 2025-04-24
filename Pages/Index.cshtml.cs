using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Vacancy> FeaturedVacancies { get; set; }

        public async Task OnGetAsync()
        {
            FeaturedVacancies = await _context.Vacancies
                .Include(v => v.Company)
                .Where(v => v.Status == "Published")
                .OrderByDescending(v => v.PublishedAt)
                .Take(6)
                .ToListAsync();
        }
    }

    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value[..maxLength] + "...";
        }
    }
}