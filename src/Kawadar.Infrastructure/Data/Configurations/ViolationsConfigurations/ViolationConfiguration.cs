using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.Violations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ViolationsConfigurations
{
    public class ViolationConfiguration : IEntityTypeConfiguration<Violation>
    {
        public void Configure(EntityTypeBuilder<Violation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ViolationEvidence).HasMaxLength(500);
            builder.Property(x => x.ViolationStatus).IsRequired().HasConversion<string>();
            builder.Property(x => x.ViolationType).IsRequired().HasConversion<string>();
            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.ReferenceType).IsRequired();

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.ResolvedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
