using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class ProjectItemConfiguration : IEntityTypeConfiguration<PortfolioItem>
    {
        public void Configure(EntityTypeBuilder<PortfolioItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.UpdatedAt).IsRequired();

            builder.Property(i => i.ItemType).HasConversion<string>().IsRequired();

            builder.Property(i => i.Content).HasMaxLength(300).IsRequired();

            builder.HasOne(i => i.PortfolioProject)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.PortfolioProjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
