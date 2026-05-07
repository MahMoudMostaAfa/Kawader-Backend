namespace Kawadar.Api.Requests.Wallet;

public class CreateWithdrawalRequest
{
  public decimal Amount { get; set; }
  public Guid PayoutAccountId { get; set; }
}
