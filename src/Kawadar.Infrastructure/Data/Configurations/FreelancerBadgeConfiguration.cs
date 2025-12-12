using Kawadar.Domain.Badges;
using Kawadar.Domain.Badges.FreelancerBadges;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class FreelancerBadgeConfiguration : IEntityTypeConfiguration<FreelancerBadge>
    {
        public void Configure(EntityTypeBuilder<FreelancerBadge> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasOne<Badge>()
                .WithOne().
                HasForeignKey<FreelancerBadge>(f => f.BadgeId)
                .IsRequired().
                OnDelete(DeleteBehavior.Cascade);

            // a Foreign key with Freelancer 
        }
    }
}
