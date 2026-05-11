using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payments;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IPaymentRepository
{
  Task<Result<PaymentTransaction>> GetByIdAsync(Guid paymentTransactionId, CancellationToken cancellationToken = default);

  Task<Result<PaymentTransaction>> GetByGatewayTransactionIdAsync(string gatewayTransactionId, CancellationToken cancellationToken = default);

  Task<Result<PaymentTransaction>> GetByGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default);

  Task<List<PaymentTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

  void Add(PaymentTransaction paymentTransaction);

  void AddEventHook(PaymentEventHook eventHook);

  Task<bool> EventHookExistsAsync(string gatewayEventId, CancellationToken cancellationToken = default);
}
