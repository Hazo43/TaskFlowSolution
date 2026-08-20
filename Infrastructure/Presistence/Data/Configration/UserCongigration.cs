using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configration
{
    public class UserCongigration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.DisplayName)
                   .HasMaxLength(100);


            // Project 
            builder.HasMany(u => u.Projects)
                   .WithOne(u => u.Owner)
                   .HasForeignKey(u => u.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ProjectMember
            builder.HasMany(x => x.ProjectMembers)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Task
            builder.HasMany(x => x.Tasks)
                   .WithOne(x => x.AssignedTo)
                   .HasForeignKey(x => x.AssignedToId)
                   .OnDelete(DeleteBehavior.SetNull);

            // comment
            builder.HasMany(x => x.Comments)
                   .WithOne(x => x.Author)
                   .HasForeignKey(x => x.AuthorId)
                   .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
