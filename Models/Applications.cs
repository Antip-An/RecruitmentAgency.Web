using System;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentAgency.Web.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string CoverLetter { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        // Foreign keys
        public int VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }

        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public string? ReviewedById { get; set; }
        public User? ReviewedBy { get; set; }
    }
}