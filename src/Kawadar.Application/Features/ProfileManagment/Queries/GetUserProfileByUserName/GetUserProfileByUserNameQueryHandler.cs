using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfileByUserName;


public class GetUserProfileByUserNameQueryHandler : IRequestHandler<GetUserProfileByUserNameQuery, Result<UserProfileDto>>
{
  private readonly IMapper _mapper;
  private readonly IUser _user;
  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;

  public GetUserProfileByUserNameQueryHandler(IMapper mapper, IUser user, IIdentityService identityService, IUsersRepository usersRepository)
  {
    _mapper = mapper;
    _user = user;
    _identityService = identityService;
    _usersRepository = usersRepository;
  }
  public async Task<Result<UserProfileDto>> Handle(GetUserProfileByUserNameQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    // 1- check that current the user is exist 
    var userResult = await _identityService.GetUserByIdAsync(userId);

    if (userResult.IsError) return userResult.Errors;


    // 2 - get the userId of the requested user profile
    var requestedUserResult = await _identityService.GetUserByUserNameAsync(request.UserName);

    if (requestedUserResult.IsError) return requestedUserResult.Errors;


    // 3- get user profile by the id 

    var requestedUserProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(requestedUserResult.Value.Id);

    if (requestedUserProfileResult.IsError) return requestedUserProfileResult.Errors;

    var userProfile = requestedUserProfileResult.Value;


    if ((userProfile.IsActivated is false && userId != requestedUserResult.Value.Id) || (userProfile.ProfileType is ProfileType.Client && userId != requestedUserResult.Value.Id) || userProfile.IsBanned is true || userProfile.IsDeleted is true || userProfile.IsAvailable is false) return Error.NotFound("UserProfile.NotFound", "The Requested UserProfile Not Found");


    var userProfileDto = _mapper.Map<UserProfileDto>((userProfile, requestedUserResult.Value));


    if (userId != requestedUserResult.Value.Id) userProfileDto.LastName = userProfileDto.LastName.Substring(0, 1);
    if (userId != requestedUserResult.Value.Id) userProfileDto.PhoneNumber = null;




    return userProfileDto;
  }
}