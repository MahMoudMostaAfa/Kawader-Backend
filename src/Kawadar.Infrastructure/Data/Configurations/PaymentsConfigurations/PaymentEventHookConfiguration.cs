using Kawadar.Domain.WalletAndPayments.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.PaymentsConfigurations;

public class PaymentEventHookConfiguration : IEntityTypeConfiguration<PaymentEventHook>
{
  public void Configure(EntityTypeBuilder<PaymentEventHook> builder)
  {
    builder.HasKey(peh => peh.Id);

    builder.HasOne<PaymentTransaction>()
           .WithMany()
           .HasForeignKey(peh => peh.PaymentTransactionId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.Property(peh => peh.GatewayEventId)
           .HasMaxLength(100)
           .IsRequired();

    builder.Property(peh => peh.EventType)
           .HasMaxLength(100)
           .IsRequired();

    builder.Property(peh => peh.RawPayload)
           .IsRequired();

    builder.Property(peh => peh.HMACSignature)
           .HasMaxLength(500);
  }
}
