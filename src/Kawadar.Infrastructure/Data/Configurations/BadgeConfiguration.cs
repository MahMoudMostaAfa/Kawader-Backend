using Kawadar.Domain.Badges;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
    {
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title).HasMaxLength(50).IsRequired();

            builder.Property(b => b.UpdatedAt).IsRequired();

            builder.Property(b => b.IconUrl).HasMaxLength(300).IsRequired();

            builder.Property(b => b.Description).HasMaxLength(300).IsRequired();

            builder.Property(b => b.CreatedAt).IsRequired();
        }
    }
}
