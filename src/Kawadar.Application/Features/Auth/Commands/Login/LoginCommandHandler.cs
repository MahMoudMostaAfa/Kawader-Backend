using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
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
  public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    var LoginResult = await _identityService.LoginAsync(request.Email, request.Password);
    if (LoginResult.IsError) return LoginResult.Errors;

    var userDto = LoginResult.Value;

    var tokenResult = await _tokenProvider.GenerateTokenAsync(userDto.Id);
    if (tokenResult.IsError) return tokenResult.Errors;

    return tokenResult.Value;
  }
}