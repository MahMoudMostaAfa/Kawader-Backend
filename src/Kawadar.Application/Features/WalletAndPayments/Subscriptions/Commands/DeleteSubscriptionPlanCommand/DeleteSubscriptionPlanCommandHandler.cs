using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.DeleteSubscriptionPlanCommand
{
    public class DeleteSubscriptionPlanCommandHandler(IUser user, ISubscriptionsRepository subscriptionsRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteSubscriptionPlanCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var plan = await subscriptionsRepository.GetSubscriptionPlanById(request.Id);
            if (plan.IsError) return plan.Errors;

            var deletionResult = subscriptionsRepository.RemoveSubscriptionPlan(plan.Value);
            if (deletionResult.IsError) return deletionResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Deleted;
        }
    }
}
