using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Domain.WalletAndPayments.Payouts;




public abstract class PayoutAccountDetails;
public class MobileWalletAccountDetails : PayoutAccountDetails
{
  public MobileWalletProvider Provider { get; set; }      // "Vodafone Cash", "Orange Money", "Etisalat Cash"
  public string PhoneNumber { get; set; } = null!;
}

public class BankTransferAccountDetails : PayoutAccountDetails
{
  public string BankName { get; set; } = null!;
  public string AccountHolderName { get; set; } = null!;
  public string AccountNumber { get; set; } = null!;
  public string? IBAN { get; set; }
  public string? SwiftCode { get; set; }
}

public class InstaPayAccountDetails : PayoutAccountDetails
{
  public string IPA { get; set; } = null!;          // InstaPay Address e.g. username@instapay
}