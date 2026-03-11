using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginDto>>
{

  private readonly IIdentityService _identityService;
  private readonly ITokenProvider _tokenProvider;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;

  public LoginCommandHandler(
    IIdentityService identityService,
    ITokenProvider tokenProvider,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork)
  {
    _identityService = identityService;
    _tokenProvider = tokenProvider;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    var LoginResult = await _identityService.LoginAsync(request.Email, request.Password);
    if (LoginResult.IsError) return LoginResult.Errors;

    var userDto = LoginResult.Value;

    // Check if user profile is soft-deleted and cancel the scheduled deletion
    var profileResult = await _usersRepository.GetUserProfileByUserIdAsync(userDto.Id);
    if (!profileResult.IsError && profileResult.Value.IsDeleted)
    {
      profileResult.Value.CancelDeletion();
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

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