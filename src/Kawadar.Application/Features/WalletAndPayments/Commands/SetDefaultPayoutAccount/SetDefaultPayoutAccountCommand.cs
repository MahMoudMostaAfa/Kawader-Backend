using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.SetDefaultPayoutAccount;

public record SetDefaultPayoutAccountCommand(Guid AccountId) : IRequest<Result<Updated>>;
