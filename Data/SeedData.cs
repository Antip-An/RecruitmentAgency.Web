using Microsoft.AspNetCore.Identity;
using RecruitmentAgency.Web.Models;
using RecruitmentAgency.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecruitmentAgency.Web.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            // Проверяем подключение к БД
            if (!await context.Database.CanConnectAsync())
            {
                throw new Exception("Не удалось подключиться к базе данных");
            }

            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager, roleManager, context);
            await SeedTestCompanyAsync(context, userManager);
            await SeedSkillsAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
        {
            var roles = new[]
            {
                new Role("Admin") { Description = "Администратор системы" },
                new Role("Manager") { Description = "Менеджер по подбору" },
                new Role("Employer") { Description = "Работодатель" },
                new Role("JobSeeker") { Description = "Соискатель" }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Ошибка создания роли {role.Name}: {string.Join(", ", result.Errors)}");
                    }
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<Role> roleManager, AppDbContext context)
        {
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "System",
                    EmailConfirmed = true,
                    IsActive = true
                };

                await userManager.CreateAsync(adminUser, adminPassword);
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var adminRole = await roleManager.FindByNameAsync("Admin");
                if (adminRole != null)
                {
                    context.UserRoles.Add(new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id,
                        AssignedDate = DateTime.UtcNow,
                        AssignedBy = "System"
                    });
                    await context.SaveChangesAsync();
                }

                await userManager.UpdateSecurityStampAsync(adminUser);
            }
        }

        private static async Task SeedTestCompanyAsync(AppDbContext context, UserManager<User> userManager)
        {
            if (!await context.Companies.AnyAsync())
            {
                var admin = await userManager.FindByEmailAsync("admin@example.com");
                if (admin == null) return;

                var company = new Company
                {
                    Name = "ТехноКорп",
                    Description = "IT-решения для бизнеса",
                    Website = "https://techcorp.example.com",
                    LogoUrl = "/images/logo.png",
                    ContactEmail = "hr@techcorp.example.com",
                    CreatedBy = admin
                };

                await context.Companies.AddAsync(company);
                await context.SaveChangesAsync();

                var vacancies = new[]
                {
                    new Vacancy
                    {
                        Title = "Senior .NET Developer",
                        Description = "Разработка корпоративных решений",
                        Requirements = "Опыт работы от 5 лет",
                        SalaryFrom = 200000,
                        SalaryTo = 350000,
                        Location = "Москва",
                        IsRemote = true,
                        Status = "Published",
                        Company = company,
                        CreatedBy = admin
                    }
                };

                await context.Vacancies.AddRangeAsync(vacancies);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedSkillsAsync(AppDbContext context)
        {
            if (!await context.Skills.AnyAsync())
            {
                var skills = new[]
                {
                    new Skill { Name = "C#" },
                    new Skill { Name = "ASP.NET Core" },
                    new Skill { Name = "SQL" },
                    new Skill { Name = "JavaScript" },
                    new Skill { Name = "HTML/CSS" },
                    new Skill { Name = "React" },
                    new Skill { Name = "Angular" },
                    new Skill { Name = "Docker" },
                    new Skill { Name = "Git" }
                };

                await context.Skills.AddRangeAsync(skills);
                var saved = await context.SaveChangesAsync();

                if (saved == 0)
                {
                    throw new Exception("Навыки не были сохранены в базу данных");
                }
            }
        }
    }
}