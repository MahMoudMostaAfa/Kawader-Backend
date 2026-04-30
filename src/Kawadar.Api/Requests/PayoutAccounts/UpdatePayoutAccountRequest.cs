using Kawadar.Domain.WalletAndPayments.Payouts;

namespace Kawadar.Api.Requests.PayoutAccounts;

public class UpdatePayoutAccountRequest
{
  public string DisplayName { get; set; } = null!;

  /// <summary>
  /// Structured account details object.
  /// Send one of: MobileWalletAccountDetails, BankTransferAccountDetails, or InstaPayAccountDetails.
  /// </summary>
  public PayoutAccountDetails AccountDetails { get; set; } = null!;
  public bool IsDefault { get; set; }
}
