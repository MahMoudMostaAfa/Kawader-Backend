using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Portfolios.ProjectView;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class PortfolioProjectViewConfiguration : IEntityTypeConfiguration<PortfolioProjectView>
    {
        public void Configure(EntityTypeBuilder<PortfolioProjectView> builder)
        {
            builder.HasKey(p => p.Id);


            builder.HasOne(p => p.PortfolioProject)
                .WithMany()
                .HasForeignKey(p => p.PortfolioProjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(i => i.UserProfile)
                .WithMany()
                .HasForeignKey(p => p.UserProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
