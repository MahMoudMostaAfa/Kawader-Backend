using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
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
  private readonly IRecommendationService _recommendationService;
  private readonly ISkillRepository _skillRepository;
  private readonly ISpecilizationRepository _specilizationRepository;

  public UpdateProfileCommandHandler(IUser user, IIdentityService identityService, IUsersRepository usersRepository, IUnitOfWork unitOfWork, IRecommendationService recommendationService, ISkillRepository skillRepository, ISpecilizationRepository specilizationRepository)
  {
    _user = user;
    _identityService = identityService;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
    _recommendationService = recommendationService;
    _skillRepository = skillRepository;
    _specilizationRepository = specilizationRepository;
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

    // Update user labels in Gorse with skills, specialization, experience, and profile type
    var skillNames = await _skillRepository.GetFreelancerSkillsByUserProfileId(userProfile.Id);
    var labels = skillNames
      .Select(s => s.ToLower())
      .Concat(new[] { userProfile.ExperienceYear.ToString().ToLower(), userProfile.ProfileType.ToString().ToLower() })
      .ToList();

    if (userProfile.SpecializationId.HasValue)
    {
      var specResult = await _specilizationRepository.GetById(userProfile.SpecializationId.Value);
      if (!specResult.IsError)
        labels.Add(specResult.Value.Name.ToLower());
    }

    await _recommendationService.UpdateUserAsync(
      userProfile.Id,
      labels: labels.ToArray(),
      comment: userProfile.FullName,
      ct: cancellationToken);

    return Result.Updated;

  }
}

