using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Application.Features.WalletAndPayments.DTOs;

public class WithdrawalRequestDto
{
  public Guid Id { get; set; }
  public Guid WalletId { get; set; }
  public Guid PayoutAccountId { get; set; }
  public decimal Amount { get; set; }
  public string Currency { get; set; } = string.Empty;
  public WithdrawalStatus Status { get; set; }
  public string? FailureReason { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public DateTime? ProcessedAt { get; set; }
  public Guid? WalletTransactionId { get; set; }
}
