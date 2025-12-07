using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    internal class BadgeRepository(AppDbContext appDbContext) : IBadgeRepository
    {
        public async Task<Result<Success>> AddAsync(Badge badge)
        {
            await appDbContext.Badges.AddAsync(badge);
            return Result.Success;
        }

        public Result<Deleted> Delete(Badge badge)
        {
            appDbContext.Badges.Remove(badge);
            return Result.Deleted;
        }

        public async Task<Result<Badge>> GetById(Guid Id)
        {
            var badge = await appDbContext.Badges.FirstOrDefaultAsync(b => b.Id == Id);
            return badge;
        }

        public async Task<Result<Updated>> Update(Guid BadgeId, Badge NewBadge)
        {
            var badge = await appDbContext.Badges.FirstOrDefaultAsync(b => b.Id == BadgeId);
            badge.Update(NewBadge.iconUrl);
            return Result.Updated;
        }
    }
}
