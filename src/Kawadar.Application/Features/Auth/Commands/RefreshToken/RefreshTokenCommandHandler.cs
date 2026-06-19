using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
{

  private readonly IIdentityService _identityService;
  private readonly ITokenProvider _tokenProvider;

  private IUser _user;

  public RefreshTokenCommandHandler(IIdentityService identityService, ITokenProvider tokenProvider, IUser user)
  {
    _identityService = identityService;
    _tokenProvider = tokenProvider;
    _user = user;

  }
  public async Task<Result<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var refreshTokenResult = await _identityService.GetRefreshTokenAsync(userId);

    if (refreshTokenResult.IsError) return refreshTokenResult.Errors;

    if (refreshTokenResult.Value.RefreshToken != request.RefreshToken) return Error.Validation("Auth.RefreshToken", "Invalid refresh token");

    if (refreshTokenResult.Value.Expires <= DateTime.UtcNow) return Error.Validation("Auth.RefreshToken", "Refresh token has expired");

    // Generate new access token
    var newAccessTokenResult = await _tokenProvider.GenerateTokenAsync(userId);
    if (newAccessTokenResult.IsError) return newAccessTokenResult.Errors;
    var newAccessToken = newAccessTokenResult.Value;

    // Generate new refresh token
    var newRefreshTokenResult = _tokenProvider.GenerateRefreshTokenAsync();
    if (newRefreshTokenResult.IsError) return newRefreshTokenResult.Errors;
    var newRefreshToken = newRefreshTokenResult.Value;

    // Update refresh token in database

    var updateRefreshTokenResult = await _identityService.AddRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(7));
    if (updateRefreshTokenResult.IsError) return updateRefreshTokenResult.Errors;


    var response = new RefreshTokenResponseDto
    {
      AccessToken = newAccessToken,
      RefreshToken = newRefreshToken
    };

    return response;
  }
}