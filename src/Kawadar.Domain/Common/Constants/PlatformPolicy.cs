public static class PlatformPolicy
{
  public static decimal PlatformFeePercentage => 0.1m;
  public static TimeSpan EscrowReleaseDelay => TimeSpan.FromMinutes(2);
}