namespace Kawadar.Application.Features.WalletAndPayments.DTOs;

public class AdminWalletDto
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public decimal Balance { get; set; }
  public decimal EscrowBalance { get; set; }
  public decimal TotalBalance { get; set; }
  public string Currency { get; set; } = string.Empty;
  public bool IsActive { get; set; }
}
