
namespace RecruitmentAgency.Web.Models
{
    public class JobSeeker
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string ProfessionalTitle { get; set; }
        public string Summary { get; set; }
        public string Education { get; set; }
        public List<JobSeekerSkill> Skills { get; set; } = new();
        public List<Application> Applications { get; set; } = new();
    }
}

// JobSeekerSkill.cs
namespace RecruitmentAgency.Web.Models
{
    public class JobSeekerSkill
    {
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
        
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}