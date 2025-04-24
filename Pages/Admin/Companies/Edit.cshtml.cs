using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Companies
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Название обязательно")]
            [MaxLength(255, ErrorMessage = "Максимальная длина 255 символов")]
            public string Name { get; set; }

            public string Description { get; set; }

            [Url(ErrorMessage = "Некорректный URL")]
            [MaxLength(255, ErrorMessage = "Максимальная длина 255 символов")]
            public string Website { get; set; }

            [Url(ErrorMessage = "Некорректный URL")]
            [MaxLength(512, ErrorMessage = "Максимальная длина 512 символов")]
            [Display(Name = "Ссылка на логотип")]
            public string LogoUrl { get; set; }

            [Required(ErrorMessage = "Контактный email обязателен")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            [MaxLength(255, ErrorMessage = "Максимальная длина 255 символов")]
            [Display(Name = "Контактный email")]
            public string ContactEmail { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Website = company.Website,
                LogoUrl = company.LogoUrl,
                ContactEmail = company.ContactEmail
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var company = await _context.Companies.FindAsync(Input.Id);
            if (company == null)
            {
                return NotFound();
            }

            company.Name = Input.Name;
            company.Description = Input.Description;
            company.Website = Input.Website;
            company.LogoUrl = Input.LogoUrl;
            company.ContactEmail = Input.ContactEmail;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Компания успешно обновлена";
            return RedirectToPage("./Index");
        }
    }
}