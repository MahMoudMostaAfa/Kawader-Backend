using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CreateSubscriptionPlan
{
    public class CreateSubscriptionPlanCommandHandler(IUser user, ISubscriptionsRepository subscriptionsRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateSubscriptionPlanCommand, Result<Created>>
    {
        public async Task<Result<Created>> Handle(CreateSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var Planfeature = new PlanFeatures
            {
                ProposalsPerMonth = request.proposalsPerMonth,
                TotalProtfolioProjects = request.TotalPortfolioProjects,
                TwentyFourSevenSupport = request.TwentyFourSevenSupport
            };

            var subscriptionPlan = SubscriptionPlan.Create(request.name, request.price, request.CycleType, Planfeature);
            if (subscriptionPlan.IsError) return subscriptionPlan.Errors;

            await subscriptionsRepository.AddSubscriptionPlan(subscriptionPlan.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Created;
        }
    }
}
