using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IWalletRepository
{
  Task<Result<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

  Task<Result<EscrowTransaction>> GetEscrowTransactionByContractId(Guid contractId, CancellationToken cancellationToken);

  void AddEscrowTransaction(EscrowTransaction transaction);

  void AddWalletTransaction(WalletTransaction transaction);

}