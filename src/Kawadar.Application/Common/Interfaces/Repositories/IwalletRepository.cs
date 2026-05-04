using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IWalletRepository
{
  Task<Result<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<Result<EscrowTransaction>> GetEscrowTransactionByContractId(Guid contractId, CancellationToken cancellationToken = default);
  Task<Result<EscrowTransaction>> GetEscrowTransactionById(Guid escrowTransactionId, CancellationToken cancellationToken = default);
  void AddEscrowTransaction(EscrowTransaction transaction);

  void AddWalletTransaction(WalletTransaction transaction);

}