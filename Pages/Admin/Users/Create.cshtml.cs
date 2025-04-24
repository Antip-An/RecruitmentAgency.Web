using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentAgency.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public CreateModel(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList Roles { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Имя")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Пароль должен содержать минимум {2} и максимум {1} символов.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Роли")]
            public string[] SelectedRoles { get; set; }
        }

        public void OnGet()
        {
            var allRoles = _roleManager.Roles;
            Roles = new SelectList(allRoles, "Name", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var allRoles = _roleManager.Roles;
                Roles = new SelectList(allRoles, "Name", "Name");
                return Page();
            }

            var user = new User
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                if (Input.SelectedRoles != null && Input.SelectedRoles.Length > 0)
                {
                    await _userManager.AddToRolesAsync(user, Input.SelectedRoles);
                }

                return RedirectToPage("./Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var roles = _roleManager.Roles;
            Roles = new SelectList(roles, "Name", "Name");
            return Page();
        }
    }
}