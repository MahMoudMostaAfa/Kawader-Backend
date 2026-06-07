using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.PaymentsConfigurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
  public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
  {
    builder.HasKey(pt => pt.Id);

    builder.HasOne<UserProfile>()
           .WithMany()
           .HasForeignKey(pt => pt.UserId)
           .OnDelete(DeleteBehavior.NoAction);

    builder.HasOne<Wallet>()
           .WithMany()
           .HasForeignKey(pt => pt.WalletId)
           .OnDelete(DeleteBehavior.NoAction);

    builder.HasOne<WalletTransaction>()
           .WithOne()
           .HasForeignKey<PaymentTransaction>(pt => pt.WalletTransactionId)
           .OnDelete(DeleteBehavior.SetNull)
           .IsRequired(false)
           ;

    builder.Property(pt => pt.Amount)
           .HasColumnType("decimal(18,2)")
           .IsRequired();

    builder.Property(pt => pt.Currency)
           .HasMaxLength(3)
           .IsRequired();

    builder.Property(pt => pt.Gateway)
           .HasConversion<string>()
           .IsRequired();

    builder.Property(pt => pt.Method)
           .HasConversion<string>()
           .IsRequired();

    builder.Property(pt => pt.Status)
           .HasConversion<string>()
           .IsRequired();





  }
}