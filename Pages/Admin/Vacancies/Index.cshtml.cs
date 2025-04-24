using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Vacancies
{
    [Authorize(Roles = "Admin,Manager")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Vacancy> Vacancies { get; set; }

        public async Task OnGetAsync()
        {
            Vacancies = await _context.Vacancies
                .Include(v => v.Company)
                .Include(v => v.CreatedBy)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var vacancy = await _context.Vacancies
                .Include(v => v.Applications)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vacancy == null)
            {
                return NotFound();
            }

            // Проверяем есть ли отклики
            if (vacancy.Applications?.Any() == true)
            {
                TempData["ErrorMessage"] = "Невозможно удалить вакансию, так как есть связанные отклики";
                return RedirectToPage();
            }

            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Вакансия успешно удалена";
            return RedirectToPage();
        }
    }
}