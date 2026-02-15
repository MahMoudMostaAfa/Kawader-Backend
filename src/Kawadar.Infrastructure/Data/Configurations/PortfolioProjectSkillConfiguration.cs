using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Portfolios.ProjectSkill;
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
                .WithOne()
                .HasForeignKey<PortfolioProjectSkill>(p => p.PortfolioProjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // a Foreign key with the skill entity
        }
    }
}
