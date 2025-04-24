using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Vacancies
{
    [Authorize(Roles = "Admin,Manager,Employer")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public CreateModel(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList Companies { get; set; }
        public MultiSelectList Skills { get; set; } // Добавлен список навыков

        public class InputModel
        {
            [Required(ErrorMessage = "Название обязательно")]
            [MaxLength(255, ErrorMessage = "Максимальная длина 255 символов")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Описание обязательно")]
            public string Description { get; set; }

            public string Requirements { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Значение должно быть положительным")]
            public decimal? SalaryFrom { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Значение должно быть положительным")]
            public decimal? SalaryTo { get; set; }

            [MaxLength(255, ErrorMessage = "Максимальная длина 255 символов")]
            public string Location { get; set; }

            public bool IsRemote { get; set; }

            [Required(ErrorMessage = "Статус обязателен")]
            public string Status { get; set; }

            [Required(ErrorMessage = "Компания обязательна")]
            public int CompanyId { get; set; }

            [Display(Name = "Требуемые навыки")]
            public List<int> SelectedSkillIds { get; set; } = new(); // Для хранения выбранных навыков
        }

        public async Task OnGetAsync()
        {
            await LoadSelectLists();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Создаем вакансию
            var vacancy = new Vacancy
            {
                Title = Input.Title,
                Description = Input.Description,
                Requirements = Input.Requirements,
                SalaryFrom = Input.SalaryFrom,
                SalaryTo = Input.SalaryTo,
                Location = Input.Location,
                IsRemote = Input.IsRemote,
                Status = Input.Status,
                CompanyId = Input.CompanyId,
                CreatedById = currentUser.Id,
                CreatedAt = DateTime.UtcNow,
                PublishedAt = Input.Status == "Published" ? DateTime.UtcNow : null,
                VacancySkills = new List<VacancySkill>() // Инициализируем коллекцию навыков
            };

            // Добавляем выбранные навыки к вакансии
            if (Input.SelectedSkillIds != null && Input.SelectedSkillIds.Any())
            {
                foreach (var skillId in Input.SelectedSkillIds)
                {
                    vacancy.VacancySkills.Add(new VacancySkill
                    {
                        SkillId = skillId,
                        Vacancy = vacancy
                    });
                }
            }

            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync();

            // Перенаправление в зависимости от роли
            if (User.IsInRole("Employer") || User.IsInRole("Manager"))
            {
                return RedirectToPage("/Vacancies/Index");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Admin/Vacancies/Index");
            }

            TempData["SuccessMessage"] = "Вакансия успешно создана";
            return RedirectToPage("/Index");
        }

        private async Task LoadSelectLists()
        {
            // Загрузка компаний
            Companies = new SelectList(
                await _context.Companies.OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name");

            // Загрузка навыков из базы данных
            var skills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
            Skills = new MultiSelectList(skills, "Id", "Name");
        }
    }
}