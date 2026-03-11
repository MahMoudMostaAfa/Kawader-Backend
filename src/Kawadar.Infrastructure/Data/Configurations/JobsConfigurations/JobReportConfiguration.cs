using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.JobsConfigurations
{
    public class JobReportConfiguration : IEntityTypeConfiguration<JobReport>
    {
        public void Configure(EntityTypeBuilder<JobReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ReportStatus).HasConversion<string>().IsRequired();
            builder.Property(x => x.ReportType).HasConversion<string>().IsRequired();
            builder.Property(x => x.Content).HasMaxLength(500).IsRequired();

            builder.HasOne<Job>()
                .WithMany()
                .HasForeignKey(x => x.JobId)
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
