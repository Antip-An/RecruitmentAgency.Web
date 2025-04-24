using Microsoft.AspNetCore.Identity;

namespace RecruitmentAgency.Web.Models
{
    public class Role : IdentityRole<string>
    {
        public Role() => Id = Guid.NewGuid().ToString();
        public Role(string roleName) : this() => Name = roleName;

        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }

    public class UserRole : IdentityUserRole<string>
    {
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public string AssignedBy { get; set; } = "System"; // Установите значение по умолчанию

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}