using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Domain.WalletAndPayments.Payouts;

public class WithdrawalRequest : AuditableEntity
{
  public Guid WalletId { get; private set; }
  public Guid UserPayoutAccountId { get; private set; }
  public decimal Amount { get; private set; }
  public string Currency { get; private set; } = "EGP"; // e.g. "USD", "EUR", "EGP"

  public WithdrawalStatus Status { get; private set; } = WithdrawalStatus.Pending;

  public string? FailureReason { get; private set; } // Populated if Status is Failed, e.g. "Insufficient funds", "Invalid payout account", etc.
  public DateTime? ProcessedAt { get; private set; } // Timestamp when the withdrawal was processed (either completed or failed)
  public Guid? ProcessedBy { get; private set; } // Admin user ID who processed the withdrawal (if applicable)

  public Guid? WalletTransactionId { get; private set; } // Link to the corresponding wallet transaction (if applicable) , put after processing the withdrawal and creating the wallet transaction to link them together

  private WithdrawalRequest()
  { } // For EF Core


  private WithdrawalRequest(Guid walletId, Guid userPayoutAccountId, decimal amount) : base(Guid.NewGuid())
  {
    WalletId = walletId;
    UserPayoutAccountId = userPayoutAccountId;
    Amount = amount;

  }

  public static Result<WithdrawalRequest> Create(Guid walletId, Guid userPayoutAccountId, decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    return new WithdrawalRequest(walletId, userPayoutAccountId, amount);
  }

  public Result<Updated> MarkAsCompleted(Guid walletTransactionId, Guid processedBy)
  {
    Status = WithdrawalStatus.Processed;
    ProcessedAt = DateTime.UtcNow;
    ProcessedBy = processedBy;
    WalletTransactionId = walletTransactionId;

    return Result.Updated;
  }

  public Result<Updated> MarkAsFailed(string failureReason, Guid processedBy)
  {
    Status = WithdrawalStatus.Rejected;
    FailureReason = failureReason;
    ProcessedAt = DateTime.UtcNow;
    ProcessedBy = processedBy;

    return Result.Updated;
  }

  public Result<Updated> ChangeStatus(WithdrawalStatus newStatus)
  {
    Status = newStatus;
    return Result.Updated;

  }


}