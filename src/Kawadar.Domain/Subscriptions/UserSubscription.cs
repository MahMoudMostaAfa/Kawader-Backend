using System.Diagnostics.Tracing;
using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Subscriptions;


public class UserSubscription : AuditableEntity
{
  public Guid UserId { get; private set; }
  public Guid SubscriptionPlanId { get; private set; }

  public UserSubscriptionStatus Status { get; private set; } = UserSubscriptionStatus.Active;

  public DateTime StartedAt { get; private set; } = DateTime.UtcNow;
  public DateTime ExpiresAt { get; private set; }

  public DateTime? CancalledAt { get; private set; }

  public Decimal TotalPrice { get; private set; } = 0m;

  public bool AutoRenew { get; private set; } = true;


  private UserSubscription()
  {
  }

  private UserSubscription(Guid userId, Guid subscriptionPlanId, DateTime expiresAt, bool autoRenew, Decimal totalPrice)
  : base(Guid.NewGuid())
  {
    UserId = userId;
    SubscriptionPlanId = subscriptionPlanId;
    ExpiresAt = expiresAt;
    AutoRenew = autoRenew;
    TotalPrice = totalPrice;

  }

  public static Result<UserSubscription> Create(Guid userId, Guid subscriptionPlanId, DateTime expiresAt, bool autoRenew, Decimal totalPrice)
  {
    var userSubscription = new UserSubscription(userId, subscriptionPlanId, expiresAt, autoRenew, totalPrice);

    return userSubscription;
  }

    public Result<Updated> Cancel()
    {
        Status = UserSubscriptionStatus.Cancalled;
        return Result.Updated;
    }
}
