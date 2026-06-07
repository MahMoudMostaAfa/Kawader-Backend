namespace Kawadar.Infrastructure.Settings;

public class GorseSettings
{
  public const string SectionName = "Gorse";

  public string BaseUri { get; set; } = default!;
  public string ApiKey { get; set; } = default!;
  public int DefaultRecommendationCount { get; set; } = 10;
}
