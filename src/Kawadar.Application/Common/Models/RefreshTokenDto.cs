namespace Kawadar.Application.Common.Models;

public class RefreshTokenDto
{
  public string RefreshToken { get; set; } = string.Empty;
  public DateTime Expires { get; set; }
}