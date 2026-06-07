using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.ApproveWithdrawalRequest;

public record ApproveWithdrawalRequestCommand(Guid WithdrawalRequestId) : IRequest<Result<Updated>>;
