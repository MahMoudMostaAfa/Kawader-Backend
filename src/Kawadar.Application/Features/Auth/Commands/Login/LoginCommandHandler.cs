using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
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

  public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
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

    return tokenResult.Value;
  }
}