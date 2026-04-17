using Kawadar.Domain.Jobs.SavedJobs;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.JobsConfigurations;


public class SavedJobsConfiguration : IEntityTypeConfiguration<SavedJob>
{
  public void Configure(EntityTypeBuilder<SavedJob> builder)
  {

    builder.HasKey(sj => sj.Id);

    builder.HasOne(sj => sj.Job)
          .WithMany()
          .HasForeignKey(sj => sj.JobId)
          .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<UserProfile>().WithMany()
          .HasForeignKey(sj => sj.SavedById)
          .OnDelete(DeleteBehavior.NoAction);

    builder.HasIndex(sj => new { sj.JobId, sj.SavedById }).IsUnique();
  }
}