using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions;
using Kawadar.Domain.Subscriptions.Enums;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemorySubscriptionsRepository : ISubscriptionsRepository
{
    private readonly Dictionary<Guid, SubscriptionPlan> _plans = new();
    private readonly Dictionary<Guid, UserSubscription> _userSubscriptions = new();

    public Task<Result<Success>> AddSubscriptionPlan(SubscriptionPlan plan)
    {
        _plans[plan.Id] = plan;
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Result<Deleted> RemoveSubscriptionPlan(SubscriptionPlan plan)
    {
        _plans.Remove(plan.Id);
        return Result.Deleted;
    }

    public Task<Result<Success>> AddUserSubscription(UserSubscription userSubscription)
    {
        _userSubscriptions[userSubscription.Id] = userSubscription;
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<List<SubscriptionPlan>>> GetSubscriptions()
        => Task.FromResult<Result<List<SubscriptionPlan>>>(_plans.Values.ToList());

    public Task<Result<SubscriptionPlan>> GetSubscriptionPlanById(Guid id)
    {
        var found = _plans.TryGetValue(id, out var plan);
        return Task.FromResult(found
            ? (Result<SubscriptionPlan>)plan!
            : Error.NotFound("Subscription.PlanNotFound", "Subscription plan not found."));
    }

    public Task<Result<UserSubscription>> GetUserSubscriptionById(Guid id)
    {
        var found = _userSubscriptions.TryGetValue(id, out var sub);
        return Task.FromResult(found
            ? (Result<UserSubscription>)sub!
            : Error.NotFound("Subscription.NotFound", "User subscription not found."));
    }

    public Task<PaginatedList<UserSubscription>> GetAllUserSubscriptionsByUserProfileId(
        Guid userProfileId, UserSubscriptionStatus? status, int page, int pageSize, string sortBy)
    {
        var query = _userSubscriptions.Values.Where(s => s.UserId == userProfileId);
        if (status.HasValue) query = query.Where(s => s.Status == status.Value);

        var list = query.ToList();
        var paged = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<UserSubscription>(paged, list.Count, page, pageSize));
    }

    public Task<Result<UserSubscription>> GetActiveUserSubscription(Guid userProfileId)
    {
        var sub = _userSubscriptions.Values
            .FirstOrDefault(s => s.UserId == userProfileId && s.Status == UserSubscriptionStatus.Active);
        return Task.FromResult(sub is not null
            ? (Result<UserSubscription>)sub
            : Error.NotFound("Subscription.NotFound", "No active subscription found."));
    }
}
