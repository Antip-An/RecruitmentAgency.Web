using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Data;
using RecruitmentAgency.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentAgency.Web.Pages.ManageRoleRequests
{
    [Authorize(Roles = "Manager")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<RoleChangeRequest> PendingRequests { get; set; }

        public async Task OnGetAsync()
        {
            PendingRequests = await _context.RoleChangeRequests
                .Include(r => r.User)
                .Where(r => !r.IsApproved && r.ProcessedDate == null)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var request = await _context.RoleChangeRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            // Удаляем текущие роли пользователя (кроме Admin/Manager)
            var userRoles = await _userManager.GetRolesAsync(request.User);
            foreach (var role in userRoles.Where(r => r != "Admin" && r != "Manager"))
            {
                await _userManager.RemoveFromRoleAsync(request.User, role);
            }

            // Добавляем запрошенную роль
            await _userManager.AddToRoleAsync(request.User, request.RequestedRole);

            // Обновляем запрос
            request.IsApproved = true;
            request.ProcessedDate = DateTime.UtcNow;
            request.ProcessedById = _userManager.GetUserId(User);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var request = await _context.RoleChangeRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.IsApproved = false;
            request.ProcessedDate = DateTime.UtcNow;
            request.ProcessedById = _userManager.GetUserId(User);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}