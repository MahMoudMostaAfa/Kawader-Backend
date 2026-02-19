using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.VisualBasic;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<Updated>>
{
  private readonly IUser _user;

  private readonly IIdentityService _identityService;

  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;

  public UpdateProfileCommandHandler(IUser user, IIdentityService identityService, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _identityService = identityService;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<Updated>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
  {

    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;
    var userResult = await _identityService.GetUserByIdAsync(userId);
    if (userResult.IsError) return userResult.Errors;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfile = userProfileResult.Value;

    var updateResult = userProfile.UpdateProfile(request.FirstName, request.LastName, request.Title, request.Bio, request.ExperienceYear, request.IsAvailable, request.ProfileType, request.PhoneNumber);
    if (updateResult.IsError) return updateResult.Errors;

    var saveChangesResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

    if (saveChangesResult == 0) return ApplicationErrors.FailedToUpdateProfile;

    return Result.Updated;

  }
}