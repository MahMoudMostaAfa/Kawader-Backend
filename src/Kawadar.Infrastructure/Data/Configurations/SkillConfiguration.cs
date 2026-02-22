using Kawadar.Domain.Skills;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).HasMaxLength(50).IsRequired();

            builder.Property(s => s.IsActive).IsRequired();

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(s => s.CreatedBy)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}