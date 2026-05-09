using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions;
using Kawadar.Domain.Subscriptions.Enums;
using Kawadar.Domain.Subscriptions.Events;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.SubscribeToPlan
{
    public class SubscribeToPlanCommandHandler(IUser user, IUsersRepository usersRepository, ISubscriptionsRepository subscriptionsRepository
        , IUnitOfWork unitOfWork, IWalletRepository walletRepository) : IRequestHandler<SubscribeToPlanCommand, Result<Created>>
    {
        public async Task<Result<Created>> Handle(SubscribeToPlanCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var plan = await subscriptionsRepository.GetSubscriptionPlanById(request.PlanId);
            if (plan.IsError) return plan.Errors;

            var walletResult = await walletRepository.GetByUserIdAsync(userProfileResult.Value.Id, cancellationToken);
            if (walletResult.IsError) return walletResult.Errors;

            var wallet = walletResult.Value;
            var ExpiryDate = DateTime.UtcNow;

            if(plan.Value.BillingCycleType == BillingCycleType.Monthly)
            {
                ExpiryDate = ExpiryDate.AddMonths(1);
            }
            else if(plan.Value.BillingCycleType == BillingCycleType.Yearly)
            {
                ExpiryDate = ExpiryDate.AddYears(1);
            }
            else
            {
                return Error.Conflict("The Billing Cycle can only be monthly or yearly");
            }

            if(wallet.Balance < plan.Value.Price)
            {
                return Error.Conflict("Insufficent Balance");
            }
            var userSubscriptionResult = UserSubscription.Create(userProfileResult.Value.Id, plan.Value.Id, ExpiryDate, request.autoRenew, plan.Value.Price);
            if (userSubscriptionResult.IsError) return userSubscriptionResult.Errors;

            var userSubscription = userSubscriptionResult.Value;
            var subscribedToPlanEvent = new SubscribedToPlanEvent
            {
                userId = userId,
                UserProfileId = userProfileResult.Value.Id,
                UserSubscriptionId = userSubscription.Id
            };

            var transaction = wallet.AddTransaction(plan.Value.Price, TransactionType.SubscriptionCharge, WalletTransactionReferenceType.Subscription, userSubscription.Id);
            if (transaction.IsError) return transaction.Errors;

            await subscriptionsRepository.AddUserSubscription(userSubscription);
             
            walletRepository.AddWalletTransaction(transaction.Value);
            userSubscription.AddDomainEvent(subscribedToPlanEvent);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Created;
        }
    }
}