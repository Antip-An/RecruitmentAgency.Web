using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Form
{
    [Authorize(Roles = "JobSeeker")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            AppDbContext context,
            UserManager<User> userManager,
            ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public ProfileModel Profile { get; set; } = new();

        public bool HasProfile { get; set; }
        public MultiSelectList Skills { get; set; } = default!;
        public List<Application> Applications { get; set; } = new(); // Добавлено для хранения откликов

        public async Task OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    _logger.LogWarning("User not found");
                    return;
                }

                var jobSeeker = await _context.JobSeekers
                    .Include(js => js.Skills)
                    .FirstOrDefaultAsync(js => js.UserId == user.Id);

                HasProfile = jobSeeker is not null;

                await LoadSkillsAsync();

                if (HasProfile && jobSeeker is not null)
                {
                    Profile = new ProfileModel
                    {
                        Id = jobSeeker.Id,
                        ProfessionalTitle = jobSeeker.ProfessionalTitle ?? string.Empty,
                        Summary = jobSeeker.Summary ?? string.Empty,
                        Education = jobSeeker.Education ?? string.Empty,
                        SelectedSkillIds = jobSeeker.Skills?.Select(s => s.SkillId).ToList() ?? new()
                    };

                    // Загружаем отклики в отдельное свойство
                    Applications = await _context.Applications
                        .Include(a => a.Vacancy)
                            .ThenInclude(v => v.Company)
                        .Where(a => a.JobSeekerId == jobSeeker.Id)
                        .OrderByDescending(a => a.AppliedAt)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile");
                ModelState.AddModelError(string.Empty, "Ошибка загрузки анкеты");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSkillsAsync();
                return Page();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    _logger.LogWarning("User not found");
                    return NotFound();
                }

                if (Profile.Id == 0) // Create new
                {
                    var jobSeeker = new JobSeeker
                    {
                        UserId = user.Id,
                        ProfessionalTitle = Profile.ProfessionalTitle,
                        Summary = Profile.Summary,
                        Education = Profile.Education,
                        Skills = Profile.SelectedSkillIds?
                            .Select(id => new JobSeekerSkill { SkillId = id })
                            .ToList() ?? new()
                    };

                    _context.JobSeekers.Add(jobSeeker);
                }
                else // Update existing
                {
                    var jobSeeker = await _context.JobSeekers
                        .Include(js => js.Skills)
                        .FirstOrDefaultAsync(js => js.Id == Profile.Id);

                    if (jobSeeker is null)
                    {
                        _logger.LogWarning("JobSeeker profile not found");
                        return NotFound();
                    }

                    jobSeeker.ProfessionalTitle = Profile.ProfessionalTitle;
                    jobSeeker.Summary = Profile.Summary;
                    jobSeeker.Education = Profile.Education;

                    // Update skills
                    jobSeeker.Skills = Profile.SelectedSkillIds?
                        .Select(id => new JobSeekerSkill
                        {
                            JobSeekerId = Profile.Id,
                            SkillId = id
                        })
                        .ToList() ?? new();
                }

                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving profile");
                ModelState.AddModelError(string.Empty, "Ошибка сохранения анкеты");
                await LoadSkillsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            try
            {
                var jobSeeker = await _context.JobSeekers.FindAsync(Profile.Id);
                if (jobSeeker is not null)
                {
                    _context.JobSeekers.Remove(jobSeeker);
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile");
                ModelState.AddModelError(string.Empty, "Ошибка удаления анкеты");
                await LoadSkillsAsync();
                return Page();
            }
        }

        private async Task LoadSkillsAsync()
        {
            try
            {
                var skills = await _context.Skills.ToListAsync();
                Skills = new MultiSelectList(skills, "Id", "Name");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading skills");
                Skills = new MultiSelectList(Enumerable.Empty<Skill>());
            }
        }

        public class ProfileModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Укажите вашу должность")]
            public string ProfessionalTitle { get; set; } = string.Empty;

            [Required(ErrorMessage = "Заполните информацию о себе")]
            public string Summary { get; set; } = string.Empty;

            [Required(ErrorMessage = "Укажите ваше образование")]
            public string Education { get; set; } = string.Empty;

            public List<int> SelectedSkillIds { get; set; } = new();
        }
    }
}