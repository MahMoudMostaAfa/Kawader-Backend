using Kawadar.Domain.WalletAndPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.WalletConfigurations;


public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
  public void Configure(EntityTypeBuilder<WalletTransaction> builder)
  {
    builder.HasKey(wt => wt.Id);

    builder.Property(wt => wt.Id).ValueGeneratedNever();



    builder.Property(wt => wt.Type).HasConversion<string>();
    builder.Property(wt => wt.Status).HasConversion<string>();
    builder.Property(wt => wt.ReferenceType).HasConversion<string>();


    builder.Property(wt => wt.Amount).HasPrecision(18, 2);
    builder.Property(wt => wt.BalanceBefore).HasPrecision(18, 2);
    builder.Property(wt => wt.BalanceAfter).HasPrecision(18, 2);




  }
}