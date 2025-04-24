using System;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentAgency.Web.Models
{
    public class RoleChangeRequest
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
        
        [Required]
        public string RequestedRole { get; set; } // "Employer" или "JobSeeker"
        
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
        public DateTime? ProcessedDate { get; set; }
        
        public string? ProcessedById { get; set; }
        public User? ProcessedBy { get; set; }
    }
}