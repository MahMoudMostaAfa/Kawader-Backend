using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.JobViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.JobsConfigurations;

public class JobViewConfiguration : IEntityTypeConfiguration<JobView>
{
  public void Configure(EntityTypeBuilder<JobView> builder)
  {
    builder.HasKey(v => v.Id);

    builder.HasOne(v => v.Job)
      .WithMany()
      .HasForeignKey(v => v.JobId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(v => v.UserProfile)
      .WithMany()
      .HasForeignKey(v => v.UserProfileId)
      .IsRequired()
      .OnDelete(DeleteBehavior.NoAction);

    builder.HasIndex(v => new { v.JobId, v.UserProfileId });
  }
}
