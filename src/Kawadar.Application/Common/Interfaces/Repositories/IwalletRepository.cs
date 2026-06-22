using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IWalletRepository
{
  Task<Result<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
  Task<Result<Wallet>> GetByIdAsync(Guid walletId, CancellationToken cancellationToken = default);

  Task<PaginatedList<Wallet>> GetWalletsAsync(
    Guid? userId,
    bool? isActive,
    decimal? minBalance,
    decimal? maxBalance,
    int page,
    int pageSize,
    string sortBy,
    CancellationToken cancellationToken = default);

  Task<Result<EscrowTransaction>> GetEscrowTransactionByContractId(Guid contractId, CancellationToken cancellationToken = default);
  Task<Result<EscrowTransaction>> GetEscrowTransactionByContractMilestoneId(Guid contractMilestoneId, CancellationToken cancellationToken = default);
  Task<Result<EscrowTransaction>> GetEscrowTransactionById(Guid escrowTransactionId, CancellationToken cancellationToken = default);
  void AddEscrowTransaction(EscrowTransaction transaction);

  void AddWalletTransaction(WalletTransaction transaction);

    Task<PaginatedList<WalletTransaction>> GetAllTransactionsByWalletId(Guid walletId, TransactionType? type, WalletTransactionStatus? status, WalletTransactionReferenceType? reference,
        int page, int pageSize, string sortBy, CancellationToken cancellationToken);
    Task<PaginatedList<WalletTransaction>> GetAllTransactions(TransactionType? type, WalletTransactionStatus? status, WalletTransactionReferenceType? reference,
        int page, int pageSize, string sortBy, CancellationToken cancellationToken);

    Task<decimal> GetTotalProfit();
    Task<decimal> GetTotalProfitByWalletId(Guid walletId, CancellationToken cancellationToken = default);
    Task<Dictionary<WalletTransactionStatus, int>> GetTransactionStatusDistribution();
    Task<Dictionary<string, decimal>> GetMoneyTransactionDistributionBasedOnCurrency();
    Task<decimal> GetTotalEscrow();
    Task<decimal> GetTotalBalance();
  void Add(Wallet wallet);

}