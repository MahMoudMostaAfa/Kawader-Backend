using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Success>>
{

  private readonly IIdentityService _identityService;
  private readonly IUser _user;
  public LogoutCommandHandler(IIdentityService identityService, IUser user)
  {
    _identityService = identityService;
    _user = user;

  }

  public async Task<Result<Success>> Handle(LogoutCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var expireRefreshTokenResult = await _identityService.ExpireRefreshTokenAsync(userId);
    if (expireRefreshTokenResult.IsError) return expireRefreshTokenResult.Errors;

    return Result.Success;
  }
}