using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Vacancies
{
    [Authorize(Roles = "Admin,Manager")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList Companies { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

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
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }

            await LoadCompanies();

            Input = new InputModel
            {
                Id = vacancy.Id,
                Title = vacancy.Title,
                Description = vacancy.Description,
                Requirements = vacancy.Requirements,
                SalaryFrom = vacancy.SalaryFrom,
                SalaryTo = vacancy.SalaryTo,
                Location = vacancy.Location,
                IsRemote = vacancy.IsRemote,
                Status = vacancy.Status,
                CompanyId = vacancy.CompanyId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCompanies();
                return Page();
            }

            var vacancy = await _context.Vacancies.FindAsync(Input.Id);
            if (vacancy == null)
            {
                return NotFound();
            }

            // Сохраняем предыдущий статус для проверки
            var previousStatus = vacancy.Status;

            vacancy.Title = Input.Title;
            vacancy.Description = Input.Description;
            vacancy.Requirements = Input.Requirements;
            vacancy.SalaryFrom = Input.SalaryFrom;
            vacancy.SalaryTo = Input.SalaryTo;
            vacancy.Location = Input.Location;
            vacancy.IsRemote = Input.IsRemote;
            vacancy.Status = Input.Status;
            vacancy.CompanyId = Input.CompanyId;

            // Обновляем PublishedAt если статус изменился на Published
            if (Input.Status == "Published" && previousStatus != "Published")
            {
                vacancy.PublishedAt = DateTime.UtcNow;
            }
            else if (Input.Status != "Published" && previousStatus == "Published")
            {
                vacancy.PublishedAt = null;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Вакансия успешно обновлена";
            return RedirectToPage("./Index");
        }

        private async Task LoadCompanies()
        {
            Companies = new SelectList(
                await _context.Companies.OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name");
        }
    }
}