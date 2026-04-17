using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Specilizations;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class PortfolioProjectConfiguration : IEntityTypeConfiguration<PortfolioProject>
    {
        public void Configure(EntityTypeBuilder<PortfolioProject> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title).IsRequired().HasMaxLength(50);

            builder.Property(p => p.Description).IsRequired().HasMaxLength(300);

            builder.Property(p => p.CreatedAt).IsRequired();

            builder.Property(p => p.UpdatedAt).IsRequired();

            builder.Property(p => p.ProjectUrl).HasMaxLength(200);

            builder.Property(p => p.ProjectImageUrl).HasMaxLength(200);

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(p => p.FreelancerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Specilization>()
                .WithMany()
                .HasForeignKey(p => p.SpecilizationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
