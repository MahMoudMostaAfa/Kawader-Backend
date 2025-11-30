using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Common.Behaviours;

public class UnHandledExceptionBehaviour<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
  private readonly ILogger<TRequest> _logger = logger;
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    try
    {
      return await next(cancellationToken);
    }
    catch (Exception ex)
    {
      var requestName = typeof(TRequest).Name;
      _logger.LogError(ex, "Unhandled exception for request {RequestName} {@Request}", requestName, request);
      throw;
    }
  }
}