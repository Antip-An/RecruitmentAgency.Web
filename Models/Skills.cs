using System.ComponentModel.DataAnnotations;
namespace RecruitmentAgency.Web.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } // "C#", "JavaScript" и т.д.

        public List<VacancySkill> VacancySkills { get; set; }
        public List<JobSeekerSkill> JobSeekerSkills { get; set; } = new();
    }
}