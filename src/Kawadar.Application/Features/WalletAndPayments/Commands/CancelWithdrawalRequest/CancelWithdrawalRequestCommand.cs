using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CancelWithdrawalRequest;

public record CancelWithdrawalRequestCommand(Guid WithdrawalRequestId) : IRequest<Result<Deleted>>;
