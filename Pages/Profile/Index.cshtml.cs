using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public IndexModel(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public User CurrentUser { get; set; }
        public string Role { get; set; }
        public bool HasPendingRequest { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            CurrentUser = user;
            var roles = await _userManager.GetRolesAsync(user);
            Role = roles.FirstOrDefault();

            // Проверяем наличие активного запроса на смену роли
            HasPendingRequest = await _context.RoleChangeRequests
                .AnyAsync(r => r.UserId == user.Id && !r.IsApproved && r.ProcessedDate == null);

            return Page();
        }

        public async Task<IActionResult> OnPostRequestRoleChangeAsync(string requestedRole)
        {
            if (requestedRole != "Employer" && requestedRole != "JobSeeker")
            {
                TempData["ErrorMessage"] = "Некорректная роль";
                return RedirectToPage();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Проверяем, нет ли уже активного запроса
            var existingRequest = await _context.RoleChangeRequests
                .FirstOrDefaultAsync(r => r.UserId == user.Id && !r.IsApproved && r.ProcessedDate == null);

            if (existingRequest != null)
            {
                TempData["ErrorMessage"] = "У вас уже есть активный запрос на смену роли";
                return RedirectToPage();
            }

            // Проверяем текущую роль
            var currentRoles = await _userManager.GetRolesAsync(user);
            if ((requestedRole == "Employer" && currentRoles.Contains("Employer")) ||
                (requestedRole == "JobSeeker" && currentRoles.Contains("JobSeeker")))
            {
                TempData["ErrorMessage"] = "У вас уже есть эта роль";
                return RedirectToPage();
            }

            var request = new RoleChangeRequest
            {
                UserId = user.Id,
                RequestedRole = requestedRole,
                RequestDate = System.DateTime.UtcNow
            };

            _context.RoleChangeRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Запрос на смену роли отправлен администратору";
            return RedirectToPage();
        }
    }
}