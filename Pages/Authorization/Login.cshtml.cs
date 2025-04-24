using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecruitmentAgency.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email обязателен")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Находим пользователя по email
                var user = await _userManager.FindByEmailAsync(Input.Email);
                
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Неверные учетные данные");
                    return Page();
                }

                // Пробуем войти
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    // Обновляем security stamp и claims
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.RefreshSignInAsync(user);
                    
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Аккаунт заблокирован");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неверные учетные данные");
                    return Page();
                }
            }

            return Page();
        }
    }
}