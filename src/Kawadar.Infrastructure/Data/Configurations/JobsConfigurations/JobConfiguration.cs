using Kawadar.Domain.Jobs;
using Kawadar.Domain.Skills;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.JobsConfigurations;


public class JobConfiguration : IEntityTypeConfiguration<Job>
{
  public void Configure(EntityTypeBuilder<Job> builder)
  {
    // properties
    builder.HasKey(j => j.Id);
    builder.Property(j => j.Title).HasMaxLength(255).IsRequired();
    builder.Property(j => j.Description).HasMaxLength(2000).IsRequired();
    builder.HasIndex(j => j.JobSlug).IsUnique();
    builder.Property(j => j.JobSlug).HasMaxLength(255).IsRequired();
    builder.Property(j => j.JobType).HasConversion<string>().IsRequired();
    builder.Property(j => j.BudgetRange).HasConversion<string>().IsRequired();
    builder.Property(j => j.ExperienceLevel).HasConversion<string>().IsRequired();
    builder.Property(j => j.HourlyRateRange).HasConversion<string>().IsRequired();
    builder.Property(j => j.JobStatus).HasConversion<string>().IsRequired();
    builder.Property(j => j.DurationInDays).IsRequired();


    // relationships
    builder.HasMany(j => j.Questions)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);
    builder.HasMany(j => j.Attachments)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(j => j.Skills)
        .WithMany()
        .UsingEntity<Dictionary<string, object>>(
            "JobSkills",
            r => r.HasOne<Skill>().WithMany().HasForeignKey("SkillsId").OnDelete(DeleteBehavior.Restrict),
            l => l.HasOne<Job>().WithMany().HasForeignKey("JobId").OnDelete(DeleteBehavior.Cascade)
        );

    builder.HasOne(J => J.Specilization).WithMany().HasForeignKey(j => j.SpecilizationId);

    builder.HasOne<UserProfile>().WithOne().HasForeignKey<Job>(j => j.PostedById).OnDelete(DeleteBehavior.Cascade);




  }
}