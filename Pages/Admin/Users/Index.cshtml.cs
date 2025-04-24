using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecruitmentAgency.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public IndexModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IList<UserWithRoles> Users { get; set; } = new List<UserWithRoles>();

        public class UserWithRoles
        {
            public User User { get; set; }
            public IList<string> Roles { get; set; }
        }

        public async Task OnGetAsync()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserWithRoles { User = user, Roles = roles });
            }
        }

        // public async Task<IActionResult> OnPostDeleteAsync(string id)
        // {
        //     var user = await _userManager.FindByIdAsync(id);
        //     if (user != null)
        //     {
        //         await _userManager.DeleteAsync(user);
        //     }
        //     return RedirectToPage();
        // }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Проверяем, не пытаемся ли удалить сами себя
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] = "Вы не можете удалить свой собственный аккаунт!";
                return RedirectToPage();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Ошибка при удалении пользователя: " +
                    string.Join(", ", result.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["SuccessMessage"] = "Пользователь успешно удален";
            }

            return RedirectToPage();
        }
    }
}