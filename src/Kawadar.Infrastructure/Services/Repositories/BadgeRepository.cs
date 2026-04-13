using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Badges.FreelancerBadges;
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

            if (badge == null) return Error.NotFound("Badge.NotFound", "Badge not found");
            return badge;
        }

        public async Task<Result<Success>> AddBadgeToFreelancer(FreelancerBadge freelancerBadge)
        {
            await appDbContext.FreelancerBadges.AddAsync(freelancerBadge);
            return Result.Success;
        }

        public async Task<IEnumerable<Badge>> GetAllFreelancerBadges(Guid FreelancerId)
        {
            var FreelancerBadges = await (from b in appDbContext.Badges
                                          join fb in appDbContext.FreelancerBadges on b.Id equals fb.BadgeId
                                          where fb.FreelancerId == FreelancerId
                                          select b).ToListAsync();
            return FreelancerBadges;
        }

        public async Task<IEnumerable<Badge>> GetAllBadges()
        {
            var Badges = await appDbContext.Badges.ToListAsync();
            return Badges;
        }
    }
}
