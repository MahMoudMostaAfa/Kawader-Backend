using Kawadar.Domain.Common;

namespace Kawadar.Domain.Subscriptions.Events
{
    public class SubscribedToPlanEvent : DomainEvent
    {
        public string userId { get; set; } = "";
        public Guid UserProfileId { get; set; }
        public Guid UserSubscriptionId { get; set; }

    }
}
