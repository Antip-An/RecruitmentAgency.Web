using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Vacancies
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private const int PageSize = 6;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
        public List<Company> Companies { get; set; } = new List<Company>();

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CompanyId { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? Remote { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies.ToListAsync();

            var query = _context.Vacancies
                .Include(v => v.Company)
                .Where(v => v.Status == "Published")
                .AsQueryable();

            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(v => 
                    v.Title.Contains(Search) || 
                    v.Description.Contains(Search));
            }

            if (CompanyId.HasValue)
            {
                query = query.Where(v => v.CompanyId == CompanyId);
            }

            if (Remote.HasValue && Remote.Value)
            {
                query = query.Where(v => v.IsRemote);
            }

            var count = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            Vacancies = await query
                .OrderByDescending(v => v.PublishedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}