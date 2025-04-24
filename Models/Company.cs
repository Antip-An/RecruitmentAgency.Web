using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentAgency.Web.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Url, MaxLength(255)]
        public string Website { get; set; }

        [Url, MaxLength(512)]
        public string LogoUrl { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string ContactEmail { get; set; }

        // Relationships
        public string CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public List<Vacancy> Vacancies { get; set; }
    }
}