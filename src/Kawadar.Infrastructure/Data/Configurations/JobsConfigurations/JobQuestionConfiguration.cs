using Kawadar.Domain.Jobs.JobQuestions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.JobsConfigurations;


public class JobQuestionConfiguration : IEntityTypeConfiguration<JobQuestion>
{
  public void Configure(EntityTypeBuilder<JobQuestion> builder)
  {
    builder.HasKey(jq => jq.Id);
    builder.Property(jq => jq.Question).HasMaxLength(1000).IsRequired();
    builder.Property(jq => jq.IsRequired).IsRequired();
    builder.Property(jq => jq.DisplayOrder).IsRequired();
  }
}