using System.Text.Json;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using Kawadar.Domain.WalletAndPayments.Payments;
using Kawadar.Domain.WalletAndPayments.Payments.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.ProcessPaymobCallback;

public class ProcessPaymobCallbackCommandHandler
  : IRequestHandler<ProcessPaymobCallbackCommand, Result<Success>>
{
  private readonly IPaymobService _paymobService;
  private readonly IPaymentRepository _paymentRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<ProcessPaymobCallbackCommandHandler> _logger;

  public ProcessPaymobCallbackCommandHandler(
    IPaymobService paymobService,
    IPaymentRepository paymentRepository,
    IWalletRepository walletRepository,
    IUnitOfWork unitOfWork,
    ILogger<ProcessPaymobCallbackCommandHandler> logger)
  {
    _paymobService = paymobService;
    _paymentRepository = paymentRepository;
    _walletRepository = walletRepository;
    _unitOfWork = unitOfWork;
    _logger = logger;
  }

  public async Task<Result<Success>> Handle(ProcessPaymobCallbackCommand request, CancellationToken cancellationToken)
  {
    try
    {
      using var doc = JsonDocument.Parse(request.RawPayload);
      var root = doc.RootElement;

      // Paymob sends the transaction data inside "obj"
      var obj = root.GetProperty("obj");

      var transactionId = obj.GetProperty("id").ToString();
      var orderId = obj.GetProperty("order").GetProperty("id").ToString();
      var amountCents = obj.GetProperty("amount_cents").ToString();
      var success = obj.GetProperty("success").GetBoolean();
      var pending = obj.GetProperty("pending").GetBoolean();
      var internalOrderId = obj.GetProperty("order").GetProperty("merchant_order_id").ToString();
            var hasParentTransaction = obj.GetProperty("has_parent_transaction").GetBoolean();

            // 1. Idempotency check — skip if already processed
            var alreadyProcessed = await _paymentRepository.EventHookExistsAsync(transactionId);
      if (alreadyProcessed)
      {
        _logger.LogInformation("Paymob callback for transaction {TransactionId} already processed. Skipping.", transactionId);
        return Result.Success;
      }

      // 2. Extract HMAC verification data
      var callbackData = new PaymobCallbackData(
        AmountCents: amountCents,
        CreatedAt: obj.GetProperty("created_at").GetString() ?? string.Empty,
        Currency: obj.GetProperty("currency").GetString() ?? "EGP",
        ErrorCode: obj.TryGetProperty("error_occured", out var errProp) ? errProp.ToString() : string.Empty,
        has_parent_transaction: hasParentTransaction,
        transactionId: transactionId,
        IntegrationId: obj.GetProperty("integration_id").ToString(),
        Is3dSecure: obj.GetProperty("is_3d_secure").GetBoolean(),
        IsAuth: obj.GetProperty("is_auth").GetBoolean(),
        IsCapture: obj.GetProperty("is_capture").GetBoolean(),
        IsRefunded: obj.GetProperty("is_refunded").GetBoolean(),
        IsStandalonePayment: obj.GetProperty("is_standalone_payment").GetBoolean(),
        IsVoided: obj.GetProperty("is_voided").GetBoolean(),
        OrderId: orderId,
        Owner: obj.GetProperty("owner").ToString(),
        Pending: pending,
        SourceDataPan: obj.GetProperty("source_data").GetProperty("pan").GetString() ?? string.Empty,
        SourceDataSubType: obj.GetProperty("source_data").GetProperty("sub_type").GetString() ?? string.Empty,
        SourceDataType: obj.GetProperty("source_data").GetProperty("type").GetString() ?? string.Empty,
        Success: success
      );

      // 3. Verify HMAC signature
      var isValid = _paymobService.VerifyHmacSignature(callbackData, request.HmacSignature);

      // 4. Find our local PaymentTransaction
      var paymentResult = await _paymentRepository.GetByIdAsync(Guid.Parse(internalOrderId));

        // Create event hook record regardless of finding the payment
        Guid paymentTxId = paymentResult.IsError ? Guid.Empty : paymentResult.Value.Id;

      var eventHookResult = PaymentEventHook.Create(
        paymentTransactionId: paymentTxId,
        gatewayEventId: transactionId,
        eventType: success ? "TRANSACTION_SUCCESS" : "TRANSACTION_FAILED",
        rawPayload: request.RawPayload,
        hmacSignature: request.HmacSignature);

      if (eventHookResult.IsError) return eventHookResult.Errors;
      var eventHook = eventHookResult.Value;

      eventHook.SetSignatureValidationResult(isValid);

      if (!isValid)
      {
        _logger.LogWarning("Invalid HMAC signature for Paymob transaction {TransactionId}", transactionId);
        eventHook.MarkAsProcessed("Invalid HMAC signature");
        _paymentRepository.AddEventHook(eventHook);
        await _unitOfWork.SaveChangesAsync();
        return Error.Validation("Paymob.InvalidSignature", "HMAC signature verification failed.");
      }

      if (paymentResult.IsError)
      {
        _logger.LogWarning("No matching PaymentTransaction found for Paymob transaction {TransactionId}", transactionId);
        eventHook.MarkAsProcessed("No matching payment transaction found");
        _paymentRepository.AddEventHook(eventHook);
        await _unitOfWork.SaveChangesAsync();
        return Error.NotFound("PaymentTransaction.NotFound", "No matching payment transaction found.");
      }

      var paymentTx = paymentResult.Value;

      // 5. Update PaymentTransaction status
      if (success && !pending)
      {
        paymentTx.MarkAsCompleted();

        // 6. Credit the wallet — deposit the amount
        var walletResult = await _walletRepository.GetByIdAsync(paymentTx.WalletId);
        if (walletResult.IsError)
        {
          eventHook.MarkAsProcessed($"Wallet not found: {paymentTx.WalletId}");
          _paymentRepository.AddEventHook(eventHook);
          await _unitOfWork.SaveChangesAsync();
          return walletResult.Errors;
        }

        var wallet = walletResult.Value;
        var depositResult = wallet.AddTransaction(
          amount: paymentTx.Amount,
          transactionType: TransactionType.Deposit,
          referenceType: WalletTransactionReferenceType.Payment,
          referenceId: paymentTx.Id,
          note: $"Paymob deposit via card (Transaction: {transactionId})",
          status: WalletTransactionStatus.Completed);

        if (depositResult.IsError)
        {
          eventHook.MarkAsProcessed($"Wallet deposit failed: {depositResult.TopError.Description}");
          _paymentRepository.AddEventHook(eventHook);
          await _unitOfWork.SaveChangesAsync();
          return depositResult.Errors;
        }

        _logger.LogInformation(
          "Payment completed and wallet credited. PaymentTxId: {PaymentTxId}, Amount: {Amount}",
          paymentTx.Id, paymentTx.Amount);
      }
      else if (!success)
      {
        var failureReason = obj.TryGetProperty("data", out var dataProp) &&
                            dataProp.TryGetProperty("message", out var msgProp)
          ? msgProp.GetString() ?? "Payment failed"
          : "Payment failed";

        paymentTx.MarkAsFailed(failureReason);
        _logger.LogWarning("Payment failed. PaymentTxId: {PaymentTxId}, Reason: {Reason}",
          paymentTx.Id, failureReason);
      }

      eventHook.MarkAsProcessed();
      _paymentRepository.AddEventHook(eventHook);
      await _unitOfWork.SaveChangesAsync();

      return Result.Success;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, "Failed to parse Paymob callback payload");
      return Error.Validation("Paymob.InvalidPayload", "Invalid callback payload format.");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected error processing Paymob callback");
      return Error.Unexpected("Paymob.ProcessingError", "An unexpected error occurred processing the payment callback.");
    }
  }
}
