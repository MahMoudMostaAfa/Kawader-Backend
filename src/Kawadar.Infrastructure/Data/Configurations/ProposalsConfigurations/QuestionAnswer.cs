using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.QuestionAnswers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ProposalsConfigurations;


public class QuestionAnswerConfiguration : IEntityTypeConfiguration<ProposalQuestionAnswer>
{
  public void Configure(EntityTypeBuilder<ProposalQuestionAnswer> builder)
  {
    builder.HasKey(pq => pq.Id);

    builder.HasIndex(pq => new { pq.JobProposalId, pq.QuestionId }).IsUnique();


    builder.Property(pq => pq.Answer).HasMaxLength(500).IsRequired();

    builder.HasOne<JobProposal>().WithMany().HasForeignKey(pq => pq.JobProposalId);

    builder.HasOne(pq => pq.Question).WithMany().HasForeignKey(pq => pq.QuestionId);
  }
}