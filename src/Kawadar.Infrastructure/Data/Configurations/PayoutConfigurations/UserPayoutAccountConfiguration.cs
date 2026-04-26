using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.PayoutConfigurations;

public class UserPayoutAccountConfiguration : IEntityTypeConfiguration<UserPayoutAccount>
{
  public void Configure(EntityTypeBuilder<UserPayoutAccount> builder)
  {
    builder.HasKey(u => u.Id);
    builder.HasOne<UserProfile>()
       .WithMany()
       .HasForeignKey(u => u.UserId)
       .OnDelete(DeleteBehavior.Cascade);


    builder.Property(u => u.PayoutType).HasConversion<string>()
        .IsRequired();
    builder.Property(u => u.DispalyName)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(u => u.AccountDetailsJson)
      .IsRequired();

    builder.Property(u => u.IsDefault)
      .IsRequired();

    builder.Property(u => u.IsActive)
      .IsRequired();
  }
}