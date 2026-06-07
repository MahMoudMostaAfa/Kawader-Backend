
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface ISubscriptionsRepository
    {
        Task<Result<Success>> AddSubscriptionPlan(SubscriptionPlan plan);
        Result<Deleted> RemoveSubscriptionPlan(SubscriptionPlan plan);
        Task<Result<Success>> AddUserSubscription(UserSubscription userSubscription);
        Task<Result<List<SubscriptionPlan>>> GetSubscriptions();
        Task<Result<SubscriptionPlan>> GetSubscriptionPlanById(Guid Id);
        Task<Result<UserSubscription>> GetUserSubscriptionById(Guid Id);
        Task<PaginatedList<UserSubscription>> GetAllUserSubscriptionsByUserProfileId(Guid UserProfileId, UserSubscriptionStatus? status, int page, int pageSize, string sortBy);
        Task<Result<UserSubscription>> GetActiveUserSubscription(Guid UserProfileId);
    }
}
