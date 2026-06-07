using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class SubscriptionRepository(AppDbContext appDbContext) : ISubscriptionsRepository
    {
        public async Task<Result<Success>> AddSubscriptionPlan(SubscriptionPlan plan)
        {
            await appDbContext.SubscriptionPlans.AddAsync(plan);
            return Result.Success;
        }

        public async Task<Result<Success>> AddUserSubscription(UserSubscription userSubscription)
        {
            await appDbContext.UserSubscriptions.AddAsync(userSubscription);
            return Result.Success;
        }

        public async Task<Result<UserSubscription>> GetUserSubscriptionById(Guid Id)
        {
            var subscription = await appDbContext.UserSubscriptions.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (subscription is null) return Error.NotFound();
            return subscription;
        }

        public async Task<Result<SubscriptionPlan>> GetSubscriptionPlanById(Guid Id)
        {
            var plan = await appDbContext.SubscriptionPlans.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (plan is null) return Error.NotFound("This subscription plan doesn't exist");
            return plan;
        }

        public async Task<Result<List<SubscriptionPlan>>> GetSubscriptions()
        {
            var plans = await appDbContext.SubscriptionPlans.ToListAsync();
            return plans;
        }

        public async Task<Result<UserSubscription>> GetActiveUserSubscription(Guid UserProfileId)
        {
            var subscription = await appDbContext.UserSubscriptions.Where(x => x.ExpiresAt > DateTime.UtcNow).FirstOrDefaultAsync();
            if (subscription is null) return Error.NotFound("This User has no active subscription");
            return subscription;
        }

        public Result<Deleted> RemoveSubscriptionPlan(SubscriptionPlan plan)
        {
            appDbContext.SubscriptionPlans.Remove(plan);
            return Result.Deleted;
        }

        public async Task<PaginatedList<UserSubscription>> GetAllUserSubscriptionsByUserProfileId(Guid UserProfileId, UserSubscriptionStatus? status, int page, int pageSize, string sortBy)
        {
            var query = appDbContext.UserSubscriptions.AsQueryable();
            query = query.Where(x => x.UserId == UserProfileId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
            }

            query = sortBy == "oldest"
                ? query.OrderBy(j => j.CreatedAt)
                : query.OrderByDescending(j => j.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync();

            return new PaginatedList<UserSubscription>(items, totalCount, page, pageSize);
        }
    }
}
