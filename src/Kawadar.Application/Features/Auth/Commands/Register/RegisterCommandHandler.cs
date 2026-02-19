using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Auth.Dtos;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Kawadar.Domain.UserProfiles.Events;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Success>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _identityService;



  public RegisterCommandHandler(IUnitOfWork unitOfWork, IUsersRepository usersRepository, IIdentityService identityService)
  {
    _unitOfWork = unitOfWork;
    _usersRepository = usersRepository;
    _identityService = identityService;

  }
  public async Task<Result<Success>> Handle(RegisterCommand request, CancellationToken cancellationToken)
  {
    if (request.ProfileType == ProfileType.Admin) return UserProfileErrors.FreelancerOrClientOnlyCanRegister;

    var generatedUserNameResult = await _identityService.GenerateUserNameAsync(request.FirstName, request.LastName);

    if (generatedUserNameResult.IsError) return Error.Failure("User.GenerateUserNameFailed", "Failed to generate username.");

    var createUserResult = await _identityService.RegisterAsync(request.Email, generatedUserNameResult.Value, request.Password);
    if (createUserResult.IsError) return createUserResult.Errors;

    var userId = createUserResult.Value.Id;

    await _identityService.AddToRoleAsync(userId, DefaultRoles.User);




    var userProfileResult = UserProfile.create(userId, request.FirstName, request.LastName, request.ProfileType
    );

    if (userProfileResult.IsError)
    {
      await _identityService.DeleteUserAsync(userId);
      return userProfileResult.Errors;
    }

    var createProfileResult = await _usersRepository.CreateUserProfileAsync(userProfileResult.Value);
    if (createProfileResult.IsError)
    {
      await _identityService.DeleteUserAsync(userId);
      return createProfileResult.Errors;
    }

    var userProfile = userProfileResult.Value;
    userProfile.AddDomainEvent(new CreatedUserEvent(userId, request.Email, request.FirstName));

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success;


  }
}