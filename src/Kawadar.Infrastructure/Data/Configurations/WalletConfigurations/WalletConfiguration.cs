using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.WalletAndPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.WalletConfigurations;


public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
  public void Configure(EntityTypeBuilder<Wallet> builder)
  {
    builder.HasKey(w => w.Id);

    builder.HasOne<UserProfile>()
           .WithOne()
           .HasForeignKey<Wallet>(w => w.UserId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.Property(w => w.Balance).HasPrecision(18, 2);
    builder.Property(w => w.EscrowBalance).HasPrecision(18, 2);

    builder.Ignore(w => w.TotalBalance);

    builder.HasMany(w => w.Transactions)
           .WithOne(t => t.Wallet)
           .HasForeignKey(t => t.WalletId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(w => w.Transactions).HasField("_transactions")
    .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}