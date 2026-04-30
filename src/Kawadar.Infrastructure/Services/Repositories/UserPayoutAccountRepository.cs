using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class UserPayoutAccountRepository : IUserPayoutAccountRepository
{
  private readonly AppDbContext _context;

  public UserPayoutAccountRepository(AppDbContext appDbContext)
  {
    _context = appDbContext;
  }

  public void Add(UserPayoutAccount account)
  {
    _context.UserPayoutAccounts.Add(account);
  }

  public async Task<Result<UserPayoutAccount>> GetByIdAsync(Guid accountId, CancellationToken cancellationToken)
  {
    var account = await _context.UserPayoutAccounts
      .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

    if (account is null) return Error.NotFound("PayoutAccount.NotFound", "Payout account not found.");

    return account;
  }

  public async Task<Result<List<UserPayoutAccount>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var accounts = await _context.UserPayoutAccounts
      .Where(a => a.UserId == userId && a.IsActive)
      .OrderByDescending(a => a.IsDefault)
      .ThenByDescending(a => a.CreatedAt)
      .ToListAsync(cancellationToken);

    return accounts;
  }

  public async Task<Result<UserPayoutAccount>> GetDefaultByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var account = await _context.UserPayoutAccounts
      .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault && a.IsActive, cancellationToken);

    if (account is null) return Error.NotFound("PayoutAccount.NoDefault", "No default payout account found.");

    return account;
  }

  public async Task<Result<List<UserPayoutAccount>>> GetAllDefaultsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var accounts = await _context.UserPayoutAccounts
      .Where(a => a.UserId == userId && a.IsDefault && a.IsActive)
      .ToListAsync(cancellationToken);

    return accounts;
  }
}
