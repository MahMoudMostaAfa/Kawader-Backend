using Kawadar.Domain.Contracts;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.WalletAndPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.WalletConfigurations;


public class EscrowTransactionConfiguration : IEntityTypeConfiguration<EscrowTransaction>
{
       public void Configure(EntityTypeBuilder<EscrowTransaction> builder)
       {
              builder.HasKey(e => e.Id);
              builder.HasOne<Contract>()
                     .WithMany()
                     .HasForeignKey(e => e.ContractId)
                     .OnDelete(DeleteBehavior.NoAction);

              builder.HasOne<ContractMilestone>()
                      .WithMany()
                      .HasForeignKey(e => e.ContractMilestoneId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .IsRequired(false);

              builder.Property(e => e.Type)
                      .HasConversion<string>()
                      .IsRequired();

              builder.Property(e => e.Amount)
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

              builder.Property(e => e.Note)
                     .HasMaxLength(1000)
                     .IsRequired(false);

              builder.HasOne<UserProfile>()
                     .WithMany()
                     .HasForeignKey(e => e.SenderUserId)
                     .OnDelete(DeleteBehavior.NoAction);

              builder.HasOne<UserProfile>()
                    .WithMany()
                    .HasForeignKey(e => e.ReceiverUserId)
                    .OnDelete(DeleteBehavior.NoAction);



       }
}