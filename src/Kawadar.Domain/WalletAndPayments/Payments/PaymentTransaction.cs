using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payments.Enums;

namespace Kawadar.Domain.WalletAndPayments.Payments;

public class PaymentTransaction : AuditableEntity
{
  public Guid UserId { get; private set; }

  public Guid WalletId { get; private set; }

  public Guid? WalletTransactionId { get; private set; }

  public decimal Amount { get; private set; }
  public string Currency { get; private set; } = "EGP";

  public PaymentTransactionStatus Status { get; private set; } = PaymentTransactionStatus.Pending;

  public PaymentGateway Gateway { get; private set; }
  public PaymentMethod Method { get; private set; }

  public string? GatewayTransactionId { get; private set; }

  public string? GatewayOrderId { get; private set; }


  public string? FailureReason { get; private set; }

  public DateTime? PaidAt { get; private set; }


  private PaymentTransaction() { }


  private PaymentTransaction(Guid userId, Guid walletId, decimal amount, PaymentGateway gateway, PaymentMethod method, string? gatewayTransactionId, string? gatewayOrderId) : base(Guid.NewGuid())
  {
    UserId = userId;
    WalletId = walletId;
    Amount = amount;
    Gateway = gateway;
    Method = method;
    GatewayTransactionId = gatewayTransactionId;
    GatewayOrderId = gatewayOrderId;
  }



  public static Result<PaymentTransaction> Create(Guid userId, Guid walletId, decimal amount, PaymentGateway gateway, PaymentMethod method, string? gatewayTransactionId = null, string? gatewayOrderId = null)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    var transaction = new PaymentTransaction(userId, walletId, amount, gateway, method, gatewayTransactionId, gatewayOrderId);
    return transaction;
  }


  public void MarkAsCompleted()
  {
    Status = PaymentTransactionStatus.Completed;
    PaidAt = DateTime.UtcNow;
  }

  public void MarkAsFailed(string reason)
  {
    Status = PaymentTransactionStatus.Failed;
    FailureReason = reason;
  }

  public void MarkAsExpired()
  {
    Status = PaymentTransactionStatus.Expired;
  }








}