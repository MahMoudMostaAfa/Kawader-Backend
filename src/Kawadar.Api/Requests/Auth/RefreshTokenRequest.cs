namespace Kawadar.Api.Requests.Auth;

public class RefreshTokenRequest
{
  public string AccessToken { get; set; } = null!;
}