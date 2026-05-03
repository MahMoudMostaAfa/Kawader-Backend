using System.Diagnostics;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Common.Behaviours;



public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
  private readonly ILogger<TRequest> _logger;
  private readonly IUser _user;
  private readonly TimeProvider _timeProvider;

  public PerformanceBehaviour(
      ILogger<TRequest> logger,
      IUser user,
      TimeProvider? timeProvider = null
  )
  {
    _logger = logger;
    _user = user;
    _timeProvider = timeProvider ?? TimeProvider.System;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    var start = _timeProvider.GetTimestamp();

    var response = await next();

    var elapsedMilliseconds = _timeProvider.GetElapsedTime(start).TotalMilliseconds;

    if (elapsedMilliseconds > 500)
    {
      var requestName = typeof(TRequest).Name;
      var userId = _user.Id ?? string.Empty;
      var userName = string.Empty;

      // if (!string.IsNullOrEmpty(userId))
      // {
      //   userName = await _identityService.GetUserNameAsync(userId);
      // }

      _logger.LogWarning(
          "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", requestName, elapsedMilliseconds, userId, userName, request);
    }

    return response;
  }
}