using Kawadar.Domain.WalletAndPayments.Payouts;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Api.Requests.PayoutAccounts;

public class AddPayoutAccountRequest
{
  public PayoutType PayoutType { get; set; }
  public string DisplayName { get; set; } = null!;

  /// <summary>
  /// Structured account details object. 
  /// Send one of: MobileWalletAccountDetails, BankTransferAccountDetails, or InstaPayAccountDetails
  /// depending on the PayoutType.
  /// </summary>
  public PayoutAccountDetails AccountDetails { get; set; } = null!;
  public bool IsDefault { get; set; }
}
