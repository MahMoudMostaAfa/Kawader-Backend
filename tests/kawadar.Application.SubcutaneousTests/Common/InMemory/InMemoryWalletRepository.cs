using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryWalletRepository : IWalletRepository
{
    private readonly Dictionary<Guid, Wallet> _wallets = new();
    private readonly Dictionary<Guid, EscrowTransaction> _escrowTransactions = new();
    private readonly List<WalletTransaction> _walletTransactions = new();

    public Task<Result<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var wallet = _wallets.Values.FirstOrDefault(w => w.UserId == userId);
        return Task.FromResult(wallet is not null
            ? (Result<Wallet>)wallet
            : Error.NotFound("Wallet.NotFound", "Wallet not found for this user."));
    }

    public Task<Result<Wallet>> GetByIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var found = _wallets.TryGetValue(walletId, out var wallet);
        return Task.FromResult(found
            ? (Result<Wallet>)wallet!
            : Error.NotFound("Wallet.NotFound", "Wallet not found."));
    }

    public Task<PaginatedList<Wallet>> GetWalletsAsync(
        Guid? userId, bool? isActive, decimal? minBalance, decimal? maxBalance,
        int page, int pageSize, string sortBy, CancellationToken cancellationToken = default)
    {
        var query = _wallets.Values.AsEnumerable();
        if (userId.HasValue) query = query.Where(w => w.UserId == userId.Value);
        if (isActive.HasValue) query = query.Where(w => w.IsActive == isActive.Value);
        if (minBalance.HasValue) query = query.Where(w => w.Balance >= minBalance.Value);
        if (maxBalance.HasValue) query = query.Where(w => w.Balance <= maxBalance.Value);

        var list = query.ToList();
        var paged = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<Wallet>(paged, list.Count, page, pageSize));
    }

    public Task<Result<EscrowTransaction>> GetEscrowTransactionByContractId(Guid contractId, CancellationToken cancellationToken = default)
    {
        var tx = _escrowTransactions.Values.FirstOrDefault(e => e.ContractId == contractId);
        return Task.FromResult(tx is not null
            ? (Result<EscrowTransaction>)tx
            : Error.NotFound("Escrow.NotFound", "Escrow transaction not found."));
    }

    public Task<Result<EscrowTransaction>> GetEscrowTransactionByContractMilestoneId(Guid contractMilestoneId, CancellationToken cancellationToken = default)
    {
        var tx = _escrowTransactions.Values.FirstOrDefault(e => e.ContractMilestoneId == contractMilestoneId);
        return Task.FromResult(tx is not null
            ? (Result<EscrowTransaction>)tx
            : Error.NotFound("Escrow.NotFound", "Escrow transaction not found."));
    }

    public Task<Result<EscrowTransaction>> GetEscrowTransactionById(Guid escrowTransactionId, CancellationToken cancellationToken = default)
    {
        var found = _escrowTransactions.TryGetValue(escrowTransactionId, out var tx);
        return Task.FromResult(found
            ? (Result<EscrowTransaction>)tx!
            : Error.NotFound("Escrow.NotFound", "Escrow transaction not found."));
    }

    public void AddEscrowTransaction(EscrowTransaction transaction)
        => _escrowTransactions[transaction.Id] = transaction;

    public void AddWalletTransaction(WalletTransaction transaction)
        => _walletTransactions.Add(transaction);

    public Task<PaginatedList<WalletTransaction>> GetAllTransactionsByWalletId(
        Guid walletId, TransactionType? type, WalletTransactionStatus? status,
        WalletTransactionReferenceType? reference, int page, int pageSize, string sortBy,
        CancellationToken cancellationToken)
    {
        var query = _walletTransactions.Where(t => t.WalletId == walletId).AsEnumerable();
        if (type.HasValue) query = query.Where(t => t.Type == type.Value);
        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (reference.HasValue) query = query.Where(t => t.ReferenceType == reference.Value);

        var list = query.ToList();
        var paged = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<WalletTransaction>(paged, list.Count, page, pageSize));
    }

    public Task<PaginatedList<WalletTransaction>> GetAllTransactions(
        TransactionType? type, WalletTransactionStatus? status,
        WalletTransactionReferenceType? reference, int page, int pageSize, string sortBy,
        CancellationToken cancellationToken)
    {
        var query = _walletTransactions.AsEnumerable();
        if (type.HasValue) query = query.Where(t => t.Type == type.Value);
        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (reference.HasValue) query = query.Where(t => t.ReferenceType == reference.Value);

        var list = query.ToList();
        var paged = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<WalletTransaction>(paged, list.Count, page, pageSize));
    }

    public Task<decimal> GetTotalProfit()
        => Task.FromResult(_walletTransactions.Sum(t => t.Amount));

    public Task<Dictionary<WalletTransactionStatus, int>> GetTransactionStatusDistribution()
    {
        var dist = _walletTransactions
            .GroupBy(t => t.Status)
            .ToDictionary(g => g.Key, g => g.Count());
        return Task.FromResult(dist);
    }

    public Task<Dictionary<string, decimal>> GetMoneyTransactionDistributionBasedOnCurrency()
        => Task.FromResult(new Dictionary<string, decimal> { ["EGP"] = _walletTransactions.Sum(t => t.Amount) });

    public void Add(Wallet wallet)
        => _wallets[wallet.Id] = wallet;
}
