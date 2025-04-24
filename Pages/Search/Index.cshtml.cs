using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Search
{
    [Authorize(Roles = "Employer,Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<int> SelectedSkillIds { get; set; } = new();

        public List<JobSeeker> JobSeekers { get; set; } = new();
        public MultiSelectList Skills { get; set; }

        public async Task OnGetAsync()
        {
            // Получаем все навыки для выпадающего списка
            var allSkills = await _context.Skills
                .OrderBy(s => s.Name)
                .ToListAsync();

            Skills = new MultiSelectList(allSkills, "Id", "Name");

            // Начинаем формировать запрос для поиска соискателей
            var query = _context.JobSeekers
                .Include(j => j.User)
                .Include(j => j.Skills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.Applications)
                    .ThenInclude(a => a.Vacancy)
                        .ThenInclude(v => v.Company)
                .AsQueryable();

            // Фильтрация по поисковому запросу
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(j =>
                    j.User.FirstName.Contains(SearchTerm) ||
                    j.User.LastName.Contains(SearchTerm) ||
                    (j.ProfessionalTitle != null && j.ProfessionalTitle.Contains(SearchTerm)));
            }

            // Фильтрация по навыкам
            if (SelectedSkillIds.Any())
            {
                query = query.Where(j =>
                    j.Skills.Any(js => SelectedSkillIds.Contains(js.SkillId)));
            }

            // Получаем результаты
            JobSeekers = await query
                .OrderByDescending(j => j.User.RegistrationDate)
                .ToListAsync();
        }
    }
}