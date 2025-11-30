using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger, IUser user) : IRequestPreProcessor<TRequest> where TRequest : notnull
{
  private readonly ILogger _logger = logger;
  private readonly IUser _user = user;
  // private readonly IIdentityService _identityService = identityService;

  public async Task Process(TRequest request, CancellationToken cancellationToken)
  {
    var requestName = typeof(TRequest).Name;
    var userId = _user.Id ?? string.Empty;
    string? userName = string.Empty;

    // if (!string.IsNullOrEmpty(userId))
    // {
    //   userName = await _identityService.GetUserNameAsync(userId);
    // }
    await Task.Delay(0);
    _logger.LogInformation(
        "Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userName, request);
  }
}