using Microsoft.AspNetCore.OutputCaching;

namespace Kawadar.Api.Infrastructure;

/// <summary>
/// A custom output cache policy that allows caching for authenticated requests.
/// ASP.NET Core's default policy skips caching when an Authorization header is present.
/// This policy overrides that behaviour so all authenticated users share the same cache entries.
/// </summary>
public sealed class SharedOutputCachePolicy : IOutputCachePolicy
{
  public static readonly SharedOutputCachePolicy Instance = new();

  public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
  {
    // Override the default policy that blocks caching when Authorization header is present
    context.EnableOutputCaching = true;
    context.AllowCacheLookup = true;
    context.AllowCacheStorage = true;

    return ValueTask.CompletedTask;
  }

  public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
  {
    // Ensure we're allowed to serve from cache even for authenticated requests
    context.AllowCacheLookup = true;
    return ValueTask.CompletedTask;
  }

  public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
  {
    // Force storage for successful responses — override the default policy
    // which may have disabled storage due to the Authorization header
    if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
    {
      context.AllowCacheStorage = true;
    }
    else
    {
      context.AllowCacheStorage = false;
    }

    return ValueTask.CompletedTask;
  }
}
