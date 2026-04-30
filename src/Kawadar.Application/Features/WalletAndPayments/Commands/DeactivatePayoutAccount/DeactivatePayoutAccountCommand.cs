using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.DeactivatePayoutAccount;

public record DeactivatePayoutAccountCommand(Guid AccountId) : IRequest<Result<Deleted>>;
