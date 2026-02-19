using Kawadar.Domain.Badges;
using Kawadar.Domain.Badges.FreelancerBadges;
using Kawadar.Domain.UserProfiles;
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
                .WithMany()
                .HasForeignKey(f => f.BadgeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(up => up.FreelancerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}