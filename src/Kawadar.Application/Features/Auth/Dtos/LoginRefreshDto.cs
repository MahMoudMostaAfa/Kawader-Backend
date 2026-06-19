namespace Kawadar.Application.Features.Auth.Dtos
{
  public class LoginRefreshDto
  {
    public string refreshToken { get; set; } = "";
    public string token { get; set; } = "";
    public List<string> permissions { get; set; } = new();
    public string role { get; set; } = "";
  }
}
