using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Vacancies
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public DetailsModel(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Vacancy Vacancy { get; set; }
        public bool HasApplied { get; set; }
        public Application UserApplication { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vacancy = await _context.Vacancies
                .Include(v => v.Company)
                .Include(v => v.VacancySkills)
                    .ThenInclude(vs => vs.Skill)
                .Include(v => v.Applications)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Vacancy == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var jobSeeker = await _context.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == userId);

                if (jobSeeker != null)
                {
                    UserApplication = await _context.Applications
                        .FirstOrDefaultAsync(a => a.VacancyId == id && a.JobSeekerId == jobSeeker.Id);
                    
                    HasApplied = UserApplication != null;
                }
            }

            return Page();
        }

        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> OnPostApplyAsync(int id)
        {
            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == user.Id);

            if (jobSeeker == null)
            {
                return RedirectToPage("/Form/Index");
            }

            var application = new Application
            {
                VacancyId = id,
                JobSeekerId = jobSeeker.Id,
                AppliedAt = DateTime.UtcNow,
                Status = "Pending",
                CoverLetter = $"Отклик на вакансию {vacancy.Title}"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Вы успешно откликнулись на вакансию";
            return RedirectToPage("/Form/Index");
        }

        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.VacancyId == id && a.JobSeekerId == jobSeeker.Id);

            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Отклик успешно отменен";
            }

            return RedirectToPage("/Form/Index");
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var vacancy = await _context.Vacancies
                .Include(v => v.Applications)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vacancy == null)
            {
                return NotFound();
            }

            if (vacancy.Applications?.Any() == true)
            {
                TempData["ErrorMessage"] = "Невозможно удалить вакансию, так как есть связанные отклики";
                return RedirectToPage();
            }

            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Вакансия успешно удалена";
            return RedirectToPage("/Vacancies/Index");
        }
    }
}