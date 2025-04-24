using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Companies
{
    [Authorize(Roles = "Admin")]
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

        public class InputModel
        {
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var company = new Company
            {
                Name = Input.Name,
                Description = Input.Description,
                Website = Input.Website,
                LogoUrl = Input.LogoUrl,
                ContactEmail = Input.ContactEmail,
                CreatedById = currentUser.Id
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Компания успешно создана";
            return RedirectToPage("./Index");
        }
    }
}