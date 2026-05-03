using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class WithdrawalRequestRepository : IWithdrawalRequestRepository
{
  private readonly AppDbContext _context;

  public WithdrawalRequestRepository(AppDbContext appDbContext)
  {
    _context = appDbContext;
  }

  public void Add(WithdrawalRequest request)
  {
    _context.WithdrawalRequests.Add(request);
  }

  public async Task<Result<WithdrawalRequest>> GetByIdAsync(Guid withdrawalRequestId, CancellationToken cancellationToken)
  {
    var request = await _context.WithdrawalRequests
      .FirstOrDefaultAsync(w => w.Id == withdrawalRequestId, cancellationToken);

    if (request is null) return Error.NotFound("WithdrawalRequest.NotFound", "Withdrawal request not found.");

    return request;
  }

  public async Task<Result<List<WithdrawalRequest>>> GetByWalletIdAsync(Guid walletId, WithdrawalStatus? status,
    CancellationToken cancellationToken)
  {
    var query = _context.WithdrawalRequests.AsQueryable().Where(w => w.WalletId == walletId);

    if (status.HasValue)
    {
      query = query.Where(w => w.Status == status.Value);
    }

    var requests = await query
      .OrderByDescending(w => w.CreatedAt)
      .ToListAsync(cancellationToken);

    return requests;
  }
}
