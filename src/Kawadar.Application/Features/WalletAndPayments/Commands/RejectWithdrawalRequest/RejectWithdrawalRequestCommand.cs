using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.RejectWithdrawalRequest;

public record RejectWithdrawalRequestCommand(Guid WithdrawalRequestId, string Reason) : IRequest<Result<Updated>>;
