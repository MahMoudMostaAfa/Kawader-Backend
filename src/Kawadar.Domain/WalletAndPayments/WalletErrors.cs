using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.WalletAndPayments;

public static class WalletErrors
{
  public static Error InsufficientBalance => Error.Failure("Wallet.Balance", "Insufficient balance.");
  public static Error InvalidAmount => Error.Failure("Wallet.Amount", "Amount must be greater than zero.");
  public static Error WalletNotFound => Error.NotFound("Wallet", "Wallet not found.");
  public static Error ConcurrencyConflict => Error.Conflict("Wallet.Concurrency", "The wallet was modified by another process. Please try again.");
}