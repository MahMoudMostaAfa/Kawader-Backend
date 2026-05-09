namespace Kawadar.Infrastructure.Settings;

public class PaymobSettings
{
  public const string SectionName = "Paymob";

  public string ApiKey { get; set; } = default!;
  public string PublicKey { get; set; } = default!;
  public string SecretKey { get; set; } = default!;
  public string HMAC { get; set; } = default!;
  public string BaseUrl { get; set; } = "https://accept.paymob.com";
}
