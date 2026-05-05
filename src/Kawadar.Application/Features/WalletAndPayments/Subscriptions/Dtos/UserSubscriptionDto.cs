using Kawadar.Domain.Subscriptions;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos
{
    public class UserSubscriptionDto
    {
        public Guid Id { get; set; }
        public string SubscriptionPlanTitle { get; set; } = "";
        public UserSubscriptionStatus Status { get; set; } = UserSubscriptionStatus.Active;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public DateTime? CancalledAt { get; set; }

        public Decimal TotalPrice { get; set; } = 0m;

        public bool AutoRenew { get; set; } = true;
    }
}
