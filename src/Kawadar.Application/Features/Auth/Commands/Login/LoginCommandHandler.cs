using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginDto>>
{

  private readonly IIdentityService _identityService;
  private readonly ITokenProvider _tokenProvider;
  public LoginCommandHandler(
    IIdentityService identityService
   , ITokenProvider tokenProvider
  )
  {
    _identityService = identityService;
    _tokenProvider = tokenProvider;

  }
  public async Task<Result<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    var LoginResult = await _identityService.LoginAsync(request.Email, request.Password);
    if (LoginResult.IsError) return LoginResult.Errors;

    var userDto = LoginResult.Value;

    var tokenResult = await _tokenProvider.GenerateTokenAsync(userDto.Id);
    if (tokenResult.IsError) return tokenResult.Errors;

        var claims = await _identityService.GetUserClaimsAsync(userDto.Id);
        var permissions = claims.Value.Select(x => x.Value.Substring(12)).ToList();
        var roles = await _identityService.GetUserRolesAsync(userDto.Id);

        return new LoginDto
        {
            token = tokenResult.Value,
            permissions = permissions,
            role = roles.Value[0]
        };
  }
}