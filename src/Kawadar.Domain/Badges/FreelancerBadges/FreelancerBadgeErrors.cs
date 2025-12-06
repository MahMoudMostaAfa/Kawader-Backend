using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Badges.FreelancerBadges
{
    public class FreelancerBadgeErrors
    {
        public static Error FreelancerIdIsRequired => Error.Validation("FreelancerBadge.FreelancerIdIsRequired",
            "Freelancer Id is required");

        public static Error BadgeIdIsRequired => Error.Validation("FreelancerBadge.BadgeIdIsRequired",
            "Badge Id is required");
    }
}