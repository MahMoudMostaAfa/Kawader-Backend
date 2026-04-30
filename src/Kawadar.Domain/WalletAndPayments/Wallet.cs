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

  private Result<Updated> Deposit(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    Balance += amount;


    return Result.Updated;
  }

  private Result<Updated> Withdraw(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (Balance < amount)
      return WalletErrors.InsufficientBalance;

    Balance -= amount;

    return Result.Updated;
  }

  private Result<Updated> Hold(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (Balance < amount)
      return WalletErrors.InsufficientBalance;

    Balance -= amount;
    EscrowBalance += amount;

    return Result.Updated;
  }

  private Result<Updated> Release(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (EscrowBalance < amount)
      return WalletErrors.InsufficientBalance;

    EscrowBalance -= amount;
    Balance += amount;

    return Result.Updated;
  }

  private Result<Updated> Deduct(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    if (EscrowBalance < amount)
      return WalletErrors.InsufficientBalance;

    EscrowBalance -= amount;

    return Result.Updated;
  }

  private Result<Updated> AddEscrowedAmount(decimal amount)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    EscrowBalance += amount;

    return Result.Updated;
  }
  public Result<Updated> Deactivate()
  {
    IsActive = false;
    return Result.Updated;
  }

  public Result<WalletTransaction> AddTransaction(
    decimal amount,
    TransactionType transactionType,
    WalletTransactionReferenceType referenceType,
    Guid referenceId,
    string? note = null,
    WalletTransactionStatus status = WalletTransactionStatus.Pending
    )
  {

    if (amount > Balance && (transactionType == TransactionType.Withdrawal || transactionType == TransactionType.EscrowHold))
      return WalletErrors.InsufficientBalance;

    var transactionResult = WalletTransaction.Create(Id, transactionType, amount, Balance, Balance + amount, referenceType, referenceId, note, status);
    if (transactionResult.IsError)
      return transactionResult.Errors;

    var transaction = transactionResult.Value;
    _transactions.Add(transaction);

    if (transaction.Type == TransactionType.Withdrawal || transaction.Type == TransactionType.EscrowHold) transaction.MarkCompleted();

    if (transaction.Status == WalletTransactionStatus.Completed)
    {

      if (transaction.Type == TransactionType.Deposit) Deposit(amount);
      else if (transaction.Type == TransactionType.Withdrawal) Withdraw(amount);
      else if (transaction.Type == TransactionType.EscrowHold) Hold(amount);
      else if (transaction.Type == TransactionType.EscrowRelease) Release(amount);
      else if (transaction.Type == TransactionType.SubscriptionCharge) Withdraw(amount);
      else if (transaction.Type == TransactionType.EscrowRefund) Release(amount);
      else if (transaction.Type == TransactionType.EscrowDeduction) Deduct(amount);
      else if (transaction.Type == TransactionType.EscrowAddition) AddEscrowedAmount(amount);

    }

    return transaction;

  }

  public Result<Updated> ChangeTransactionStatus(Guid transactionId, WalletTransactionStatus newStatus)
  {
    var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
    if (transaction == null)
      return Error.NotFound("Wallet.TransactionNotFound", "The specified transaction was not found in the wallet.");

    if (transaction.Status == newStatus)
      return Result.Updated;

    // Handle status change logic
    if (transaction.Status == WalletTransactionStatus.Pending && newStatus == WalletTransactionStatus.Completed)
    {
      if (transaction.Type == TransactionType.Deposit) Deposit(transaction.Amount);
      else if (transaction.Type == TransactionType.Withdrawal) Withdraw(transaction.Amount);
      else if (transaction.Type == TransactionType.EscrowHold) Hold(transaction.Amount);
      else if (transaction.Type == TransactionType.EscrowRelease) Release(transaction.Amount);
      else if (transaction.Type == TransactionType.SubscriptionCharge) Withdraw(transaction.Amount);
      else if (transaction.Type == TransactionType.EscrowRefund) Release(transaction.Amount);
      else if (transaction.Type == TransactionType.EscrowDeduction) Deduct(transaction.Amount);
      else if (transaction.Type == TransactionType.EscrowAddition) AddEscrowedAmount(transaction.Amount);
    }


    transaction.ChangeStatus(newStatus);
    return Result.Updated;
  }
}