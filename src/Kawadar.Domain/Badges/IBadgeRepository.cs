using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Badges
{
    public interface IBadgeRepository
    {
        public Task<Result<Success>> AddAsync(Badge badge);
        public Task<Result<Badge>> GetById(Guid Id);
        public Result<Deleted> Delete(Badge badge);
        public Task<Result<Updated>> Update(Guid BadgeId, Badge NewBadge);
    }
}
