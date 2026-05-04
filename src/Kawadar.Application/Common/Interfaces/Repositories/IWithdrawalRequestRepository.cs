using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IWithdrawalRequestRepository
{
  void Add(WithdrawalRequest request);

  Task<Result<WithdrawalRequest>> GetByIdAsync(Guid withdrawalRequestId, CancellationToken cancellationToken = default);

  Task<Result<List<WithdrawalRequest>>> GetByWalletIdAsync(Guid walletId, WithdrawalStatus? status = null,
    CancellationToken cancellationToken = default);
}
