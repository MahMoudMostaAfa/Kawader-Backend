using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.UserReports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations
{
    public class UserReportConfiguration : IEntityTypeConfiguration<UserReport>
    {
        public void Configure(EntityTypeBuilder<UserReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(500);
            builder.Property(x => x.ReportType).HasConversion<string>().IsRequired();
            builder.Property(x => x.ReportStatus).HasConversion<string>();

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.ReportedUser)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.ReportedBy)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
