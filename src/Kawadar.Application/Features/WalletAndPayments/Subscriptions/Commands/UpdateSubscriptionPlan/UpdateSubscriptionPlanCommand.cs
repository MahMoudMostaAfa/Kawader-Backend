using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.UpdateSubscriptionPlan
{
    public record UpdateSubscriptionPlanCommand(Guid Id, decimal price, int ProposalsPerMonth, int PortfolioProjects, bool TwentyFourSevenSupport) : IRequest<Result<Updated>>;
}
