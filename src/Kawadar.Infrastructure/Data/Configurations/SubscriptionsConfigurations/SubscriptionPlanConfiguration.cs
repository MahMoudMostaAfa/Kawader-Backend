using Kawadar.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.SubscriptionsConfigurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
  public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
  {
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Price)
    .HasPrecision(18, 2)
    .IsRequired();
    builder.Property(x => x.BillingCycleType).HasConversion<string>().IsRequired();

    builder.OwnsOne(x => x.Features, f =>
    {
      f.ToJson();
    });



  }
}