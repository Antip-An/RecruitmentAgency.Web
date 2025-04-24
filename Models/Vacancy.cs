using System.ComponentModel.DataAnnotations;

namespace RecruitmentAgency.Web.Models
{
    public class Vacancy
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Requirements { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryFrom { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryTo { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        public bool IsRemote { get; set; } = false;

        public string Status { get; set; } = "Draft"; // Draft/Published/Archived

        // Связи
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Required]
        public string CreatedById { get; set; } // UserId создателя
        public User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; } // Nullable, т.к. не всегда публикуется

        public List<Application> Applications { get; set; } // Отклики
        public List<VacancySkill> VacancySkills { get; set; } // Навыки
    }
}