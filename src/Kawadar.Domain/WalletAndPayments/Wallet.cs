using System.ComponentModel.DataAnnotations;
using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Enums;

namespace Kawadar.Domain.WalletAndPayments;


public class Wallet : AuditableEntity
{

  public Guid UserId { get; private set; }

  public decimal Balance { get; private set; } = 0m;

  public decimal EscrowBalance { get; private set; } = 0m;
  public string Currency { get; private set; } = "EGP";

  public bool IsActive { get; private set; } = true;

  public decimal TotalBalance => Balance + EscrowBalance;

  private readonly List<WalletTransaction> _transactions = [];

  public IReadOnlyList<WalletTransaction> Transactions => _transactions.AsReadOnly();

  // OPTIMISTIC CONCURRENCY CONTROL

  [Timestamp]
  public byte[] RowVersion { get; private set; } = null!;

  private Wallet() { }

  private Wallet(Guid userId) : base(Guid.NewGuid())
  {
    UserId = userId;

  }

  public static Result<Wallet> Create(Guid userId)
  {
    return new Wallet(userId);
  }

  public Result<Updated> Deposit(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    Balance += amount;


    return Result.Updated;
  }

  public Result<Updated> Withdraw(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (Balance < amount)
      return WalletErrors.InsufficientBalance;

    Balance -= amount;

    return Result.Updated;
  }

  public Result<Updated> Hold(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (Balance < amount)
      return WalletErrors.InsufficientBalance;

    Balance -= amount;
    EscrowBalance += amount;

    return Result.Updated;
  }

  public Result<Updated> Release(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (EscrowBalance < amount)
      return WalletErrors.InsufficientBalance;

    EscrowBalance -= amount;
    Balance += amount;

    return Result.Updated;
  }

  public Result<Updated> Deduct(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (EscrowBalance < amount)
      return WalletErrors.InsufficientBalance;

    EscrowBalance -= amount;

    return Result.Updated;
  }


  public Result<Updated> Deactivate()
  {
    IsActive = false;
    return Result.Updated;
  }

  public Result<WalletTransaction> AddTransaction(
    decimal amount,
    decimal balanceBefore,
    decimal balanceAfter,
    TransactionType transactionType,
    WalletTransactionReferenceType referenceType,
    Guid referenceId,
    string? note = null
    )
  {
    var transactionResult = WalletTransaction.Create(Id, transactionType, amount, balanceBefore, balanceAfter, referenceType, referenceId, note);
    if (transactionResult.IsError)
      return transactionResult.Errors;

    var transaction = transactionResult.Value;
    _transactions.Add(transaction);
    return transaction;

  }
}