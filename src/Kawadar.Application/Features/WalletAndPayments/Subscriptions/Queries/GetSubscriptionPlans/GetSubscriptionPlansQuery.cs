using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Queries.GetSubscriptionPlans
{
    public record GetSubscriptionPlansQuery() : IRequest<Result<List<SubscriptionPlanDto>>>;
}
