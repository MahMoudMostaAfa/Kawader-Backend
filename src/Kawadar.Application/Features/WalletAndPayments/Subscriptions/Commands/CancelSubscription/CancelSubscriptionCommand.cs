using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CancelSubscription
{
    public record CancelSubscriptionCommand(Guid SubscriptionId) : IRequest<Result<Updated>>;
}
