using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Badges.FreelancerBadges
{
    public class FreelancerBadge:AuditableEntity
    {
        public Guid freelancerId { get; private set; }

        public Guid badgeId { get; private set; }

        private FreelancerBadge(Guid FreelancerId, Guid BadgeId): base(Guid.NewGuid())
        {
            freelancerId = FreelancerId;
            badgeId = BadgeId;
        }

        public static Result<FreelancerBadge> Create(Guid FreelancerId, Guid BadgeId)
        {
            if(FreelancerId == Guid.Empty)
            {
                return FreelancerBadgeErrors.FreelancerIdIsRequired;
            }

            if(BadgeId == Guid.Empty)
            {
                return FreelancerBadgeErrors.BadgeIdIsRequired;
            }

            var freelancerBadge = new FreelancerBadge(FreelancerId, BadgeId);
            return freelancerBadge;
        }
    }
}
