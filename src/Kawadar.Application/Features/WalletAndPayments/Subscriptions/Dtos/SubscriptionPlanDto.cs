using Kawadar.Domain.Subscriptions;
using Kawadar.Domain.Subscriptions.Enums;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos
{
    public class SubscriptionPlanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public decimal price { get; set; }
        public BillingCycleType billingCycle { get; set; }
        public PlanFeatures plan { get; set; } = null!;
    }
}
