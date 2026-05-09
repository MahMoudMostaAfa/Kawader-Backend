using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.DeleteSubscriptionPlanCommand
{
    public record DeleteSubscriptionPlanCommand(Guid Id) : IRequest<Result<Deleted>>;
}
