using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.PayoutConfigurations;


public class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
{
  public void Configure(EntityTypeBuilder<WithdrawalRequest> builder)
  {
    builder.HasKey(w => w.Id);

    builder.HasOne<Wallet>().WithMany().HasForeignKey(w => w.WalletId).OnDelete(DeleteBehavior.NoAction);
    builder.HasOne<UserPayoutAccount>().WithMany().HasForeignKey(w => w.UserPayoutAccountId).OnDelete(DeleteBehavior.NoAction);

    builder.Property(w => w.Amount).HasColumnType("decimal(18,2)");
    builder.Property(w => w.Currency).HasMaxLength(3).IsRequired();

    builder.Property(w => w.Status).HasConversion<string>().IsRequired();
    builder.Property(w => w.FailureReason).HasMaxLength(500);

    builder.HasOne<UserProfile>().WithMany().HasForeignKey(w => w.ProcessedBy).OnDelete(DeleteBehavior.NoAction).IsRequired(false);

    builder.HasOne<WalletTransaction>().WithOne().HasForeignKey<WithdrawalRequest>(w => w.WalletTransactionId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);
  }
}