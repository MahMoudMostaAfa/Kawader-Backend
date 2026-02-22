using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class FreelacnerSkillConfiguration : IEntityTypeConfiguration<FreelancerSkill>
    {
        public void Configure(EntityTypeBuilder<FreelancerSkill> builder)
        {
            builder.HasKey(fs => fs.Id);

            builder.Property(fs => fs.CustomSkillName).HasMaxLength(50);

            builder.Property(fs => fs.SkillType).HasConversion<string>().IsRequired();

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(fs => fs.FreelancerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(fs => fs.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
