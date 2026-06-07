using System.Text.Json;
using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Domain.WalletAndPayments.Payouts;

public class UserPayoutAccount : AuditableEntity
{

  public Guid UserId { get; private set; }
  public PayoutType PayoutType { get; private set; }

  public string DispalyName { get; private set; } = null!; // e.g. "My Vodafone Cash Account", "My Bank Account"

  public string AccountDetailsJson { get; private set; } = null!; // JSON serialized PayoutAccountDetails (MobileWalletAccountDetails, BankTransferAccountDetails, or InstaPayAccountDetails)

  public bool IsDefault { get; private set; } = false; // Indicates if this is the default payout account for the user
  public bool IsActive { get; private set; } = true; // Indicates if the payout account is active or has been deactivated by the user



  public T GetDetails<T>() where T : PayoutAccountDetails => JsonSerializer.Deserialize<T>(AccountDetailsJson) ?? throw new InvalidOperationException("Failed to deserialize payout account details.");

  public Result<PayoutAccountDetails> GetDetails() => PayoutType switch
  {
    PayoutType.MobileWallet => GetDetails<MobileWalletAccountDetails>(),
    PayoutType.BankTransfer => GetDetails<BankTransferAccountDetails>(),
    PayoutType.InstaPay => GetDetails<InstaPayAccountDetails>(),
    _ => Error.Failure("Unsupported payout type.")
  };
  private UserPayoutAccount()
  { } // For EF Core


  private UserPayoutAccount(Guid userId, PayoutType payoutType, string displayName, string accountDetailsJson, bool isDefault) : base(Guid.NewGuid())
  {
    UserId = userId;
    PayoutType = payoutType;
    DispalyName = displayName;
    AccountDetailsJson = accountDetailsJson;
    IsDefault = isDefault;
  }

  public static Result<UserPayoutAccount> Create(Guid userId, PayoutType payoutType, string displayName, string accountDetailsJson, bool isDefault)
  {


    return new UserPayoutAccount(userId, payoutType, displayName, accountDetailsJson, isDefault);

  }

  public Result<Updated> Update(string displayName, string accountDetailsJson, bool isDefault)
  {
    DispalyName = displayName;
    AccountDetailsJson = accountDetailsJson;
    IsDefault = isDefault;
    return Result.Updated;
  }

  public Result<Updated> Deactivate()
  {
    IsActive = false;
    IsDefault = false;
    return Result.Updated;
  }

  public Result<Updated> SetAsDefault()
  {
    IsDefault = true;
    return Result.Updated;
  }

  public Result<Updated> ClearDefault()
  {
    IsDefault = false;
    return Result.Updated;
  }

}