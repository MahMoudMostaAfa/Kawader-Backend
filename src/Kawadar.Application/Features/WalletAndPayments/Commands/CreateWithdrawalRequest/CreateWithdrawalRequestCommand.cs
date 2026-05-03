using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CreateWithdrawalRequest;

public record CreateWithdrawalRequestCommand(decimal Amount, Guid PayoutAccountId) : IRequest<Result<Guid>>;
