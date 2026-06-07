using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.WalletAndPayments.Payments;


public class PaymentEventHook : AuditableEntity
{
  public Guid PaymentTransactionId { get; private set; }

  public string GatewayEventId { get; private set; } = null!;

  public string EventType { get; private set; } = null!;

  public string RawPayload { get; private set; } = null!;

  public bool IsProcessed { get; private set; } = false;

  public string? ProcessingError { get; private set; }

  public DateTime? ProcessedAt { get; private set; }

  public string? HMACSignature { get; private set; }

  public bool IsValidSignature { get; private set; } = false;

  private PaymentEventHook() { }

  private PaymentEventHook(Guid paymentTransactionId, string gatewayEventId, string eventType, string rawPayload, string? hmacSignature) : base(Guid.NewGuid())
  {
    PaymentTransactionId = paymentTransactionId;
    GatewayEventId = gatewayEventId;
    EventType = eventType;
    RawPayload = rawPayload;
    HMACSignature = hmacSignature;
  }

  public static Result<PaymentEventHook> Create(Guid paymentTransactionId, string gatewayEventId, string eventType, string rawPayload, string? hmacSignature = null)
  {

    var hook = new PaymentEventHook(paymentTransactionId, gatewayEventId, eventType, rawPayload, hmacSignature);
    return hook;
  }
  public void MarkAsProcessed(string? processingError = null)
  {
    IsProcessed = true;
    ProcessingError = processingError;
    ProcessedAt = DateTime.UtcNow;
  }
  public void SetSignatureValidationResult(bool isValid)
  {
    IsValidSignature = isValid;
  }


}