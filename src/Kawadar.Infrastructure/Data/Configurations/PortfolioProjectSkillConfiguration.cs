using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class PortfolioProjectSkillConfiguration : IEntityTypeConfiguration<PortfolioProjectSkill>
    {
        public void Configure(EntityTypeBuilder<PortfolioProjectSkill> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne<PortfolioProject>()
                .WithMany()
                .HasForeignKey(p => p.PortfolioProjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(pps => pps.SkillId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
