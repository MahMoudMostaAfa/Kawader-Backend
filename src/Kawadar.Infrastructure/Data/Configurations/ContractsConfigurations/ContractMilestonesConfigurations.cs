using Kawadar.Domain.Contracts;
using Kawadar.Domain.Proposals.ProposalMilestones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ContractsConfigurations;

public class ContractMilestonesConfigurations : IEntityTypeConfiguration<ContractMilestone>
{
  public void Configure(EntityTypeBuilder<ContractMilestone> builder)
  {
    builder.HasKey(cm => cm.Id);

    builder.Property(cm => cm.Amount).HasPrecision(18, 2).IsRequired();
    builder.Property(cm => cm.Description).IsRequired();
    builder.Property(cm => cm.Title).IsRequired();
    builder.Property(cm => cm.Status).HasConversion<string>().IsRequired();

    builder.HasOne<ProposalMilestone>().WithOne().HasForeignKey<ContractMilestone>(cm => cm.ProposalMilestoneId);

  }
}