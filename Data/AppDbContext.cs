using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruitmentAgency.Web.Models;

namespace RecruitmentAgency.Web.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<VacancySkill> VacancySkills { get; set; }
        public DbSet<RoleChangeRequest> RoleChangeRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Переименование стандартных таблиц Identity
            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(u => u.Id).HasColumnName("UserId");
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.Property(r => r.Id).HasColumnName("RoleId");
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
                entity.Property(uc => uc.Id).HasColumnName("ClaimId");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
                entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
                entity.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
                entity.Property(rc => rc.Id).HasColumnName("ClaimId");
            });

            // JobSeekerSkill (many-to-many)
            builder.Entity<JobSeekerSkill>()
                .ToTable("JobSeekerSkills")
                .HasKey(js => new { js.JobSeekerId, js.SkillId });

            builder.Entity<JobSeekerSkill>()
                .HasOne(js => js.JobSeeker)
                .WithMany(j => j.Skills)
                .HasForeignKey(js => js.JobSeekerId);

            builder.Entity<JobSeekerSkill>()
                .HasOne(js => js.Skill)
                .WithMany()
                .HasForeignKey(js => js.SkillId);

            // User -> JobSeeker (1-to-1)
            builder.Entity<User>()
                .HasOne(u => u.JobSeekerProfile)
                .WithOne(j => j.User)
                .HasForeignKey<JobSeeker>(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Company -> CreatedBy User
            builder.Entity<Company>()
                .HasOne(c => c.CreatedBy)
                .WithMany(u => u.CreatedCompanies)
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Vacancy -> CreatedBy User
            builder.Entity<Vacancy>()
                .HasOne(v => v.CreatedBy)
                .WithMany(u => u.CreatedVacancies)
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // VacancySkill (many-to-many)
            builder.Entity<VacancySkill>()
                .ToTable("VacancySkills")
                .HasKey(vs => new { vs.VacancyId, vs.SkillId });

            // Application relationships
            builder.Entity<Application>()
                .ToTable("Applications")
                .HasOne(a => a.Vacancy)
                .WithMany(v => v.Applications)
                .HasForeignKey(a => a.VacancyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Application>()
                .HasOne(a => a.JobSeeker)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // AuditLog -> User
            builder.Entity<AuditLog>()
                .ToTable("AuditLogs")
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Дополнительные настройки именования
            builder.Entity<Company>().ToTable("Companies");
            builder.Entity<JobSeeker>().ToTable("JobSeekers");
            builder.Entity<Skill>().ToTable("Skills");
            builder.Entity<Vacancy>().ToTable("Vacancies");
        }
    }
}