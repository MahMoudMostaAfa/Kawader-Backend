using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class WalletRepository : IWalletRepository
{
  private readonly AppDbContext _context;

  public WalletRepository(AppDbContext appDbContext)
  {
    _context = appDbContext;

  }

  public void AddEscrowTransaction(EscrowTransaction transaction)
  {
    _context.EscrowTransactions.Add(transaction);
  }

  public void AddWalletTransaction(WalletTransaction transaction)
  {
    _context.WalletTransactions.Add(transaction);
  }

  public async Task<Result<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var wallet = await _context.Wallets.Include(w => w.Transactions).FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    if (wallet is null) return Error.NotFound();

    return wallet;

  }

  public async Task<Result<Wallet>> GetByIdAsync(Guid walletId, CancellationToken cancellationToken)
  {
    var wallet = await _context.Wallets.Include(w => w.Transactions)
      .FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);
    if (wallet is null) return Error.NotFound();

    return wallet;
  }

  public async Task<PaginatedList<Wallet>> GetWalletsAsync(
    Guid? userId,
    bool? isActive,
    decimal? minBalance,
    decimal? maxBalance,
    int page,
    int pageSize,
    string sortBy,
    CancellationToken cancellationToken)
  {
    var query = _context.Wallets.AsQueryable();

    if (userId.HasValue)
    {
      query = query.Where(w => w.UserId == userId.Value);
    }

    if (isActive.HasValue)
    {
      query = query.Where(w => w.IsActive == isActive.Value);
    }

    if (minBalance.HasValue)
    {
      query = query.Where(w => w.TotalBalance >= minBalance.Value);
    }

    if (maxBalance.HasValue)
    {
      query = query.Where(w => w.TotalBalance <= maxBalance.Value);
    }

    query = sortBy == "oldest"
      ? query.OrderBy(w => w.CreatedAt)
      : query.OrderByDescending(w => w.CreatedAt);

    var totalCount = await query.CountAsync(cancellationToken);

    var items = await query
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync(cancellationToken);

    return new PaginatedList<Wallet>(items, totalCount, page, pageSize);
  }

  public async Task<Result<EscrowTransaction>> GetEscrowTransactionByContractId(Guid contractId, CancellationToken cancellationToken)
  {
    var transaction = await _context.EscrowTransactions.FirstOrDefaultAsync(et => et.ContractId == contractId);
    if (transaction is null) return Error.NotFound();
    return transaction;
  }

  public async Task<Result<EscrowTransaction>> GetEscrowTransactionByContractMilestoneId(Guid contractMilestoneId, CancellationToken cancellationToken = default)
  {
    var transaction = await _context.EscrowTransactions
      .FirstOrDefaultAsync(et => et.ContractMilestoneId == contractMilestoneId, cancellationToken);
    if (transaction is null) return Error.NotFound();
    return transaction;
  }

  public async Task<Result<EscrowTransaction>> GetEscrowTransactionById(Guid escrowTransactionId, CancellationToken cancellationToken = default)
  {
    var transaction = await _context.EscrowTransactions.FirstOrDefaultAsync(et => et.Id == escrowTransactionId);
    if (transaction is null) return Error.NotFound();
    return transaction;
  }

  public void Add(Wallet wallet)
  {
    _context.Wallets.Add(wallet);
  }
}