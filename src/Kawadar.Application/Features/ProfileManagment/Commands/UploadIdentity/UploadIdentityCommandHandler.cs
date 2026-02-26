using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Helpers;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Messaging;
using Kawadar.Application.Common.Messaging.Messages;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UploadIdentity;


public class UploadIdentityCommandHandler : IRequestHandler<UploadIdentityCommand, Result<Success>>
{
  private readonly IIdentityService _identityService;
  private readonly IUser _user;

  private readonly IUsersRepository _usersRepository;

  private readonly IEventBus _eventBus;




  public UploadIdentityCommandHandler(IIdentityService identityService, IUser user, IUsersRepository usersRepository, IEventBus eventBus)
  {

    _identityService = identityService;
    _user = user;
    _usersRepository = usersRepository;
    _eventBus = eventBus;

  }
  public async Task<Result<Success>> Handle(UploadIdentityCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userResult = await _identityService.GetUserByIdAsync(userId);
    if (userResult.IsError) return userResult.Errors;


    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    if (userProfile.IsIdentityVerified == true) return UserProfileErrors.IdentityAlreadyVerified;

    var frontImageData = await MapIFormToFileData.MapToFileData(request.FrontImage);
    var backImageData = await MapIFormToFileData.MapToFileData(request.BackImage);

    var message = new UploadIdentityMessage
    {
      UserProfileId = userProfile.Id,
      FileName = request.FrontImage.FileName,
      IdentityBackPicData = backImageData.Data,
      IdentityFrontPicData = frontImageData.Data,
      ContainerName = Containers.IdentityImages
    };

    await _eventBus.PublishAsync(message, cancellationToken);
    await _eventBus.PublishAsync(new ProcessingIdentityDataMessage
    {
      UserProfileId = userProfile.Id,
      IdentityFrontPicData = frontImageData.Data
    }, cancellationToken);

    return Result.Success;


  }
}