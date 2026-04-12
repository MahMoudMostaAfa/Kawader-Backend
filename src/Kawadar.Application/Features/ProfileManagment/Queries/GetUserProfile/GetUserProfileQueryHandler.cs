using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
  private readonly IUser _currentUser;
  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _identityService;
  private readonly ISkillRepository _skillRepository;

  private readonly IMapper _mapper;
  public GetUserProfileQueryHandler(IUser user, IUsersRepository usersRepository, IIdentityService identityService, IMapper mapper, ISkillRepository skillRepository)
  {
    _currentUser = user;
    _usersRepository = usersRepository;
    _identityService = identityService;
    _skillRepository = skillRepository;
    _mapper = mapper;

  }
  public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
  {
    var userId = _currentUser.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    // 1 check if the user exists in the identity system

    var identityUserResult = await _identityService.GetUserByIdAsync(userId);

    if (identityUserResult.IsError) return identityUserResult.Errors;

    // 2 check if the user exists in the application database
    var UserProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);

    if (UserProfileResult.IsError) return UserProfileResult.Errors;

    var UserProfile = UserProfileResult.Value;

    var userProfileDto = _mapper.Map<UserProfileDto>((UserProfile, identityUserResult.Value));
    var skills = await _skillRepository.GetFreelancerSkillsByUserProfileId(UserProfile.Id);
    userProfileDto.skills = skills;

    return userProfileDto;

    }
}