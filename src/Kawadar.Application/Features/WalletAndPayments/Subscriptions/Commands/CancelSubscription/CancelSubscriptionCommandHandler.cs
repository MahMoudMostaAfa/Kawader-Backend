using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CancelSubscription
{
    public class CancelSubscriptionCommandHandler(IUser user, IUsersRepository usersRepository, ISubscriptionsRepository subscriptionsRepository
        ,IUnitOfWork unitOfWork) : IRequestHandler<CancelSubscriptionCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var subscriptionResult = await subscriptionsRepository.GetUserSubscriptionById(request.SubscriptionId);
            if (subscriptionResult.IsError) return subscriptionResult.Errors;

            if (userProfileResult.Value.Id != subscriptionResult.Value.UserId)
                return Error.Conflict("This subscription doesn't belong to this user");

            var subscription = subscriptionResult.Value;
            subscription.Cancel();

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Updated;
        }
    }
}
