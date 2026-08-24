using Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Presistence.Data.DbContexts
{
    public class TaskFlowDbContext : IdentityDbContext<User, IdentityRole<int> , int>
    {

        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base (options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity عشان ال
            base.OnModelCreating(modelBuilder);
          
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
        }

        public DbSet<Project> Projects { get; set; } 
        public DbSet<Tasks> Tasks { get; set; } 
        public DbSet<Comment> Comments { get; set; } 
        public DbSet<Category> Categories { get; set; } 
        public DbSet<ProjectMember> ProjectMembers { get; set; } 
        
    }
}
