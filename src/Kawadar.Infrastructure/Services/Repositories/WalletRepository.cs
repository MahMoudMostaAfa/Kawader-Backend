using Kawadar.Application.Common.Interfaces.Repositories;
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
}