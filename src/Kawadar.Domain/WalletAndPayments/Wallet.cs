using System.ComponentModel.DataAnnotations;
using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.WalletAndPayments;


public class Wallet : AuditableEntity
{

  public Guid UserId { get; private set; }

  public decimal Balance { get; private set; } = 0m;

  public decimal EscrowBalance { get; private set; } = 0m;
  public string Currency { get; private set; } = "EGP";

  public bool IsActive { get; private set; } = true;

  public decimal TotalBalance => Balance + EscrowBalance;

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
}