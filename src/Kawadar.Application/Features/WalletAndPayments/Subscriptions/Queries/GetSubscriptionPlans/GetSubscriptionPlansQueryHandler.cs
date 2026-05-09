using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Queries.GetSubscriptionPlans
{
    public class GetSubscriptionPlansQueryHandler(IUser user, ISubscriptionsRepository subscriptionsRepository, IMapper mapper) : IRequestHandler<GetSubscriptionPlansQuery, Result<List<SubscriptionPlanDto>>>
    {
        public async Task<Result<List<SubscriptionPlanDto>>> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var plans = await subscriptionsRepository.GetSubscriptions();
            if (plans.IsError) return plans.Errors;

            var plansDtos = plans.Value.Select(plan => mapper.Map<SubscriptionPlanDto>(plan)).ToList();
            return plansDtos;
        }
    }
}
