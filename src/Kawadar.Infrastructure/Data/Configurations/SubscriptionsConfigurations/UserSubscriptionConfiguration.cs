using Kawadar.Domain.Subscriptions;
using Kawadar.Domain.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kawadar.Infrastructure.Data.Configurations.SubscriptionsConfigurations;


public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
  public void Configure(EntityTypeBuilder<UserSubscription> builder)
  {
    builder.HasKey(x => x.Id);


    builder.HasOne<UserProfile>().WithMany()
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<SubscriptionPlan>().WithMany()
    .HasForeignKey(x => x.SubscriptionPlanId)
    .OnDelete(DeleteBehavior.Cascade);

    builder.Property(x => x.TotalPrice)
    .HasPrecision(18, 2)
    .IsRequired();

    builder.Property(x => x.Status)
    .HasConversion<string>()
    .IsRequired();



  }
}