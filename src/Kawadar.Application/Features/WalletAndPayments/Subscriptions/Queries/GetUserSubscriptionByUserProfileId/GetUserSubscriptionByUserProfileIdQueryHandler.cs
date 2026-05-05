using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Queries.GetUserSubscriptionByUserProfileId
{
    public class GetUserSubscriptionByUserProfileIdQueryHandler(IUser user, IUsersRepository usersRepository,
        ISubscriptionsRepository subscriptionsRepository, IMapper mapper) : IRequestHandler<GetUserSubscriptionByUserProfileIdQuery, Result<PaginatedList<UserSubscriptionDto>>>
    {
        public async Task<Result<PaginatedList<UserSubscriptionDto>>> Handle(GetUserSubscriptionByUserProfileIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var userSubscriptions = await subscriptionsRepository.GetAllUserSubscriptionsByUserProfileId(userProfileResult.Value.Id,
                request.status, request.page, request.pageSize, request.sortBy);
            
            var subscriptions = await subscriptionsRepository.GetSubscriptions();
            if (subscriptions.IsError) return subscriptions.Errors;

            var subscriptionDictionary = subscriptions.Value.ToDictionary(x => x.Id);
            var userSubscriptionDtos = subscriptions.Value.Select(pair => mapper.Map<UserSubscriptionDto>(pair)).ToList();

            for(int i = 0; i < userSubscriptions.Items.Count; i++)
            {
                var userSubscription = userSubscriptions.Items[i];
                var subscriptionPlanTitle = subscriptionDictionary[userSubscription.Id].Name;
                userSubscriptionDtos[i].SubscriptionPlanTitle = subscriptionPlanTitle;
            }

            return new PaginatedList<UserSubscriptionDto>(userSubscriptionDtos, userSubscriptions.TotalCount, request.page, request.pageSize);
        }
    }
}
