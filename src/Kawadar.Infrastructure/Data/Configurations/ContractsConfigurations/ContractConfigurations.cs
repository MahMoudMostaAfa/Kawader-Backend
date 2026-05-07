using Hangfire.Common;
using Kawadar.Domain.Contracts;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.ContractsConfigurations;

public class ContractConfigurations : IEntityTypeConfiguration<Contract>
{
  public void Configure(EntityTypeBuilder<Contract> builder)
  {
    builder.HasKey(p => p.Id);
    builder.HasOne<Domain.Jobs.Job>()
    .WithOne()
    .HasForeignKey<Contract>(c => c.JobId)
    .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<JobProposal>().WithOne().HasForeignKey<Contract>(c => c.ProposalId)
    .OnDelete(DeleteBehavior.Restrict);

    builder.Property(c => c.OneTimeFixedPrice).HasColumnType("decimal(18,2)").IsRequired(false);

    builder.HasOne<UserProfile>().WithMany().HasForeignKey(c => c.ClientId)
    .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne<UserProfile>().WithMany().HasForeignKey(c => c.FreelancerId)
    .OnDelete(DeleteBehavior.Restrict);

    builder.Property(c => c.Type).HasConversion<string>();
    builder.Property(c => c.Status).HasConversion<string>();

    builder.HasMany(c => c.ContractMilestones)
      .WithOne()
      .HasForeignKey(c => c.ContractId)
      .OnDelete(DeleteBehavior.Cascade);


    builder.Navigation(p => p.ContractMilestones).HasField("_contractMilestones").UsePropertyAccessMode(PropertyAccessMode.Field);








  }
}