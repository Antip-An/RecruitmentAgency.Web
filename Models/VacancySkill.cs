namespace RecruitmentAgency.Web.Models
{
    public class VacancySkill
    {
        public int VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}