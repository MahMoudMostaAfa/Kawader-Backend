using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.SubscribeToPlan
{
    public record SubscribeToPlanCommand(Guid PlanId, bool autoRenew) : IRequest<Result<Created>>;
}
