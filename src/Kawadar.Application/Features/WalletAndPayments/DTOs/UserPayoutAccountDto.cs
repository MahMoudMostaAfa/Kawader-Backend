using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Application.Features.WalletAndPayments.DTOs;

public class UserPayoutAccountDto
{
  public Guid Id { get; set; }
  public PayoutType PayoutType { get; set; }
  public string DisplayName { get; set; } = null!;
  public Kawadar.Domain.WalletAndPayments.Payouts.PayoutAccountDetails? AccountDetails { get; set; } = null!;
  public bool IsDefault { get; set; }
  public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
