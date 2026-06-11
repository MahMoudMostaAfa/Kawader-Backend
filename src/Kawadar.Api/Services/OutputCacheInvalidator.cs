namespace Kawadar.Api.Services;

using Kawadar.Application.Common.Interfaces.Caching;
using Microsoft.AspNetCore.OutputCaching;


public class OutputCacheInvalidator : ICacheInvalidator
{
  private readonly IOutputCacheStore _cacheStore;

  public OutputCacheInvalidator(IOutputCacheStore cacheStore)
  {
    _cacheStore = cacheStore;
  }

  public async Task EvictByTagAsync(string tag, CancellationToken cancellationToken = default)
  {
    await _cacheStore.EvictByTagAsync(tag, cancellationToken);
  }
}