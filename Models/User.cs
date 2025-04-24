using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentAgency.Web.Models
{
    public class User : IdentityUser<string>
    {
        public User() => Id = Guid.NewGuid().ToString();

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual JobSeeker JobSeekerProfile { get; set; }
        public virtual ICollection<Company> CreatedCompanies { get; set; }
        public virtual ICollection<Vacancy> CreatedVacancies { get; set; }
    }
}