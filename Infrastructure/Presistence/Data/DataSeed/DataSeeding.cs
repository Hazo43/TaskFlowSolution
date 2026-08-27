using Domain.Entites;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presistence.Data.DbContexts;
using System.Text.Json;

namespace Presistence.Data.DataSeed
{
    public class DataSeeding : IDataSeeding
    {
        private readonly TaskFlowDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public DataSeeding
            (
            TaskFlowDbContext dbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager
            )
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task DataSeedAsync()
        {
            try
            {

                var pendingmigration = await _dbContext.Database.GetPendingMigrationsAsync();
                if (pendingmigration.Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }

                // 1- Category
                if (!_dbContext.Categories.Any())
                {
                    var readCategoryData = File.OpenRead("..\\Infrastructure\\Presistence\\Data\\DataSeed\\JsonData\\categories.json");
                    var CategoryData = await JsonSerializer.DeserializeAsync<List<Category>>(readCategoryData);
                    if (CategoryData is not null && CategoryData.Any())
                        await _dbContext.Categories.AddRangeAsync(CategoryData);
                }
                await _dbContext.SaveChangesAsync();

                // 2- Project
                if (!_dbContext.Projects.Any())
                {
                    var readProjectsData = File.OpenRead("..\\Infrastructure\\Presistence\\Data\\DataSeed\\JsonData\\projects.json");
                    var projectData = await JsonSerializer.DeserializeAsync<List<Project>>(readProjectsData);
                    if (projectData is not null && projectData.Any())
                        await _dbContext.Projects.AddRangeAsync(projectData);
                }
                await _dbContext.SaveChangesAsync();

                // 3- ProjectMember
                if (!_dbContext.ProjectMembers.Any())
                {
                    var readProjectMembers = File.OpenRead("..\\Infrastructure\\Presistence\\Data\\DataSeed\\JsonData\\projectmembers.json");
                    var projectmemberData = await JsonSerializer.DeserializeAsync<List<ProjectMember>>(readProjectMembers);
                    if (projectmemberData is not null && projectmemberData.Any())
                        await _dbContext.ProjectMembers.AddRangeAsync(projectmemberData);
                }
                await _dbContext.SaveChangesAsync();

                // 4- Tasks
                if (!_dbContext.Tasks.Any())
                {
                    var readTasksData = File.OpenRead("..\\Infrastructure\\Presistence\\Data\\DataSeed\\JsonData\\tasks.json");
                    var taskData = await JsonSerializer.DeserializeAsync<List<Tasks>>(readTasksData);
                    if (taskData is not null && taskData.Any())
                        await _dbContext.Tasks.AddRangeAsync(taskData);
                }
                await _dbContext.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                Console.WriteLine($" Data Seeding Failed : {ex}");
            }
        }



        public async Task SeedIdentityDataAsync()
        {
            try
            {

                // Check RoleManager and Add ( Admin , Member )
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin" });
                    await _roleManager.CreateAsync(new IdentityRole<int> { Name = "Member" });
                }
                ;

                // Check Users And Add Two User
                if (!_userManager.Users.Any())
                {
                    var userAdmin = new User()
                    {
                        DisplayName = "HazoGaber",
                        Email = "HazoGaber@gmail.com",
                        PhoneNumber = "01212131415",
                        UserName = "hazogaber",
                    };

                    var userMember = new User()
                    {
                        DisplayName = "AhmedSayed",
                        Email = "AhmedSayed@gmail.com",
                        PhoneNumber = "01012131415",
                        UserName = "ahmedsayed",
                    };

                    // Create User
                    await _userManager.CreateAsync(userAdmin, "P@sswOrd123");
                    await _userManager.CreateAsync(userMember, "P@sswOrd123");

                    // Asign Roles 
                    await _userManager.AddToRoleAsync(userAdmin, "Admin");
                    await _userManager.AddToRoleAsync(userMember, "Member");
                }

            }

            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
