namespace Kawadar.Application.Features.WalletAndPayments.DTOs;

public class WalletDto
{
  public Guid Id { get; set; }
  public decimal Balance { get; set; }
  public decimal EscrowBalance { get; set; }
  public decimal TotalBalance { get; set; }
  public decimal TotalProfit { get; set; }
}