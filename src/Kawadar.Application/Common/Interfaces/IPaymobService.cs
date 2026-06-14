using Kawadar.Domain.Common.Results;
using System.Runtime.Serialization;

namespace Kawadar.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the Paymob payment gateway.
/// Provides methods for creating payment intentions and verifying webhook callbacks.
/// </summary>
public interface IPaymobService
{
  /// <summary>
  /// Creates a payment intention on Paymob and returns the client secret
  /// needed by the frontend/SDK to launch the checkout UI.
  /// </summary>
  Task<Result<PaymobIntentionResult>> CreatePaymentIntentionAsync(
    decimal amount,
    string currency,
    List<string> paymentMethodIds,
    PaymobBillingData billingData,
    string? internalOrderId = null,
    CancellationToken ct = default);

  /// <summary>
  /// Verifies the HMAC signature of a Paymob webhook callback
  /// to ensure authenticity and data integrity.
  /// </summary>
  bool VerifyHmacSignature(PaymobCallbackData callbackData, string receivedHmac);
}


// ─── DTOs (kept in Application layer, no Paymob dependency) ───

/// <summary>Result of creating a payment intention on Paymob.</summary>
public record PaymobIntentionResult(
  string IntentionId,
  string ClientSecret,
  decimal Amount,
  string Currency);

/// <summary>Billing data required by Paymob for payment processing.</summary>
public record PaymobBillingData(
  string FirstName,
  string LastName,
  string Email,
  string PhoneNumber,
  string? Apartment = "NA",
  string? Floor = "NA",
  string? Street = "NA",
  string? Building = "NA",
  string? ShippingMethod = "NA",
  string? PostalCode = "NA",
  string? City = "NA",
  string? Country = "EGY",
  string? State = "NA");

/// <summary>Data extracted from a Paymob transaction callback for HMAC verification.</summary>
public record PaymobCallbackData(
  string AmountCents,
  string CreatedAt,
  string Currency,
  string ErrorCode,
  bool has_parent_transaction,
  string transactionId,
  string IntegrationId,
  bool Is3dSecure,
  bool IsAuth,
  bool IsCapture,
  bool IsRefunded,
  bool IsStandalonePayment,
  bool IsVoided,
  string OrderId,
  string Owner,
  bool Pending,
  string SourceDataPan,
  string SourceDataSubType,
  string SourceDataType,
  bool Success);
