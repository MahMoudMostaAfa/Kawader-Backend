using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions.Enums;

namespace Kawadar.Domain.Subscriptions;

public class SubscriptionPlan : AuditableEntity
{
  public string Name { get; private set; } = string.Empty;
  public decimal Price { get; private set; } = 0m;

  public BillingCycleType BillingCycleType { get; private set; }

  public PlanFeatures Features { get; private set; } = new();
  public bool IsActive { get; private set; } = true;

  private SubscriptionPlan()
  {
  }
  private SubscriptionPlan(string name, decimal price, BillingCycleType billingCycleType, PlanFeatures features)
  : base(Guid.NewGuid())
  {
    Features = features;
    Name = name;
    Price = price;
    BillingCycleType = billingCycleType;
  }


  public static Result<SubscriptionPlan> Create(string name, decimal price, BillingCycleType billingCycleType, PlanFeatures features)
  {
    var subscriptionPlan = new SubscriptionPlan(name, price, billingCycleType, features);

    return subscriptionPlan;
  }

    public Result<Updated> Update(decimal price, int ProposalsPerMonth, int PortfolioProjects, bool TwentyFourSevenSupport)
    {
        Price = price;
        Features.ProposalsPerMonth = ProposalsPerMonth;
        Features.TotalProtfolioProjects = PortfolioProjects;
        Features.TwentyFourSevenSupport = TwentyFourSevenSupport;
        UpdatedAt = DateTime.UtcNow;

        return Result.Updated;
    }
}