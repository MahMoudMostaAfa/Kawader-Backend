using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.ProposalMilestones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ProposalsConfigurations;

public class MilestoneConfiguration : IEntityTypeConfiguration<ProposalMilestone>
{
  public void Configure(EntityTypeBuilder<ProposalMilestone> builder)
  {
    builder.HasKey(m => m.Id);

    builder.Property(m => m.Title)
           .IsRequired()
           .HasMaxLength(255);

    builder.Property(m => m.Description)
           .HasMaxLength(2000);

    builder.Property(m => m.Amount)
             .HasPrecision(18, 2);

    builder.Property(m => m.Status).HasConversion<string>()
           .IsRequired();

    builder.HasOne<JobProposal>()
           .WithMany()
           .HasForeignKey(m => m.JobProposalId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}