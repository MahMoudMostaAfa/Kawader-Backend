using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Subscriptions.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CreateSubscriptionPlan
{
    public record CreateSubscriptionPlanCommand(string name, decimal price, BillingCycleType CycleType,
        int proposalsPerMonth, int TotalPortfolioProjects, bool TwentyFourSevenSupport) : IRequest<Result<Created>>;
}
