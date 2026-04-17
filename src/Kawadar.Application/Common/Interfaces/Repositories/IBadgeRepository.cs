using Kawadar.Domain.Badges;
using Kawadar.Domain.Badges.FreelancerBadges;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces
{
    public interface IBadgeRepository
    {
        public Task<Result<Success>> AddAsync(Badge badge);
        public Task<Result<Badge>> GetById(Guid Id);
        public Result<Deleted> Delete(Badge badge);
        public Task<IEnumerable<Badge>> GetAllFreelancerBadges(Guid FreelancerId);
        public Task<Result<Success>> AddBadgeToFreelancer(FreelancerBadge freelancerBadge);
        public Task<IEnumerable<Badge>> GetAllBadges();
    }
}