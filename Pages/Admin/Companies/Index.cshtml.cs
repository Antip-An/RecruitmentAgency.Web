using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Companies
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Company> Companies { get; set; }

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies
                .Include(c => c.CreatedBy)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var company = await _context.Companies
                .Include(c => c.Vacancies)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            // Каскадное удаление вакансий компании
            if (company.Vacancies?.Any() == true)
            {
                _context.Vacancies.RemoveRange(company.Vacancies);
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Компания и все связанные вакансии успешно удалены";
            return RedirectToPage();
        }
    }
}