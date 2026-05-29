using MediatR;

namespace Kawadar.Application.Common.Interfaces;


public interface ICachedQuery
{


  string CacheKey { get; }
  string[] Tags { get; }
  TimeSpan Expiration => TimeSpan.FromMinutes(30);
  TimeSpan LocalCacheExpiration => TimeSpan.FromSeconds(40);

}

public interface ICachedQuery<TResponse> : IRequest<TResponse>, ICachedQuery;