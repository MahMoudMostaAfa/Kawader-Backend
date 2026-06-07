using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Enums;

namespace Kawadar.Domain.WalletAndPayments;


public class WalletTransaction : AuditableEntity
{
  public Guid WalletId { get; private set; }

  public Wallet Wallet { get; private set; } = null!;

  public TransactionType Type { get; private set; }

  public WalletTransactionStatus Status { get; private set; } = WalletTransactionStatus.Pending;
  public decimal Amount { get; private set; }
  public string Currency { get; private set; } = "EGP";

  public decimal BalanceBefore { get; private set; }
  public decimal BalanceAfter { get; private set; }

  public WalletTransactionReferenceType ReferenceType { get; private set; }

  public Guid ReferenceId { get; private set; }

  public string? Note { get; private set; }

  private WalletTransaction() { }

  private WalletTransaction(Guid walletId, TransactionType type, decimal amount, decimal balanceBefore, decimal balanceAfter, WalletTransactionReferenceType referenceType, Guid referenceId, string? note = null, WalletTransactionStatus status = WalletTransactionStatus.Pending) : base(Guid.NewGuid())
  {
    WalletId = walletId;
    Type = type;
    Amount = amount;
    BalanceBefore = balanceBefore;
    BalanceAfter = balanceAfter;
    ReferenceType = referenceType;
    ReferenceId = referenceId;
    Note = note;
    Status = status;
  }


  public static Result<WalletTransaction> Create(Guid walletId, TransactionType type, decimal amount, decimal balanceBefore, decimal balanceAfter, WalletTransactionReferenceType referenceType, Guid referenceId, string? note = null, WalletTransactionStatus status = WalletTransactionStatus.Pending)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    return new WalletTransaction(walletId, type, amount, balanceBefore, balanceAfter, referenceType, referenceId, note, status);


  }

  public void MarkCompleted()
  {
    Status = WalletTransactionStatus.Completed;
  }

  public Result<Updated> ChangeStatus(WalletTransactionStatus newStatus)
  {
    if (Status == newStatus)
      return Result.Updated;

    Status = newStatus;
    return Result.Updated;
  }
}