using System;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentAgency.Web.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public string Action { get; set; }

        [MaxLength(255)]
        public string EntityType { get; set; }

        public int? EntityId { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}