using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.UpdateSubscriptionPlan
{
    public class UpdateSubscriptionPlanCommandHandler(IUser user, ISubscriptionsRepository subscriptionsRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateSubscriptionPlanCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var subscriptionPlanResult = await subscriptionsRepository.GetSubscriptionPlanById(request.Id);
            if (subscriptionPlanResult.IsError) return subscriptionPlanResult.Errors;

            var subscriptionPlan = subscriptionPlanResult.Value;

            subscriptionPlan.Update(request.price, request.ProposalsPerMonth, request.PortfolioProjects, request.TwentyFourSevenSupport);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
