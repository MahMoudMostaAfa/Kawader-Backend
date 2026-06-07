using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payments;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class PaymentRepository : IPaymentRepository
{
  private readonly AppDbContext _context;

  public PaymentRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<Result<PaymentTransaction>> GetByIdAsync(Guid paymentTransactionId, CancellationToken cancellationToken = default)
  {
    var transaction = await _context.PaymentTransactions
      .FirstOrDefaultAsync(t => t.Id == paymentTransactionId, cancellationToken);

    if (transaction is null)
      return Error.NotFound("PaymentTransaction.NotFound", "Payment transaction not found.");

    return transaction;
  }

  public async Task<Result<PaymentTransaction>> GetByGatewayTransactionIdAsync(string gatewayTransactionId, CancellationToken cancellationToken = default)
  {
    var transaction = await _context.PaymentTransactions
      .FirstOrDefaultAsync(t => t.GatewayTransactionId == gatewayTransactionId, cancellationToken);

    if (transaction is null)
      return Error.NotFound("PaymentTransaction.NotFound", "Payment transaction not found.");

    return transaction;
  }

  public async Task<Result<PaymentTransaction>> GetByGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default)
  {
    var transaction = await _context.PaymentTransactions
      .FirstOrDefaultAsync(t => t.GatewayOrderId == gatewayOrderId, cancellationToken);

    if (transaction is null)
      return Error.NotFound("PaymentTransaction.NotFound", "Payment transaction not found.");

    return transaction;
  }

  public async Task<List<PaymentTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return await _context.PaymentTransactions
      .Where(t => t.UserId == userId)
      .OrderByDescending(t => t.CreatedAt)
      .ToListAsync(cancellationToken);
  }

  public void Add(PaymentTransaction paymentTransaction)
  {
    _context.PaymentTransactions.Add(paymentTransaction);
  }

  public void AddEventHook(PaymentEventHook eventHook)
  {
    _context.PaymentEventHooks.Add(eventHook);
  }

  public async Task<bool> EventHookExistsAsync(string gatewayEventId, CancellationToken cancellationToken = default)
  {
    return await _context.PaymentEventHooks
      .AnyAsync(e => e.GatewayEventId == gatewayEventId, cancellationToken);
  }
}
