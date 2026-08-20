using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configration
{
    public class TaskCongigration : IEntityTypeConfiguration<Domain.Entites.Tasks>
    {
        public void Configure(EntityTypeBuilder<Domain.Entites.Tasks> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);
         
            builder.Property(x => x.Description)
                   .HasMaxLength(200);

            // Comment
            builder.HasMany(x => x.Comments)
                   .WithOne(x => x.Task)
                   .HasForeignKey(x => x.TaskId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Category
            builder.HasOne(x => x.Category)
                   .WithMany(x => x.Tasks)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
