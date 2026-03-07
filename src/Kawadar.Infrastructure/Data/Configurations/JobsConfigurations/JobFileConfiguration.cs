using Kawadar.Domain.Jobs.JobFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.JobsConfigurations;


public class JobFileConfiguration : IEntityTypeConfiguration<JobFile>
{
  public void Configure(EntityTypeBuilder<JobFile> builder)
  {

    builder.HasKey(jf => jf.Id);
    builder.Property(jf => jf.Id).ValueGeneratedNever();
    builder.OwnsOne(jf => jf.File, fi =>
    {
      fi.Property(f => f.FileName).HasMaxLength(255).IsRequired();
      fi.Property(f => f.FileSizeInBytes).IsRequired();
      fi.Property(f => f.FileUrl).HasMaxLength(500).IsRequired();
      fi.Property(f => f.MimeType).HasMaxLength(100).IsRequired();
    });
  }
}