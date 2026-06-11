namespace Kawadar.Application.Common.Interfaces.Caching;

public interface ICacheInvalidator
{
  Task EvictByTagAsync(string tag, CancellationToken cancellationToken = default);
}