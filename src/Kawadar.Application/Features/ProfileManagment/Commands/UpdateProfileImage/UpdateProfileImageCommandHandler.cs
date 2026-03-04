using System.ComponentModel;
using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Helpers;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Messaging;
using Kawadar.Application.Common.Messaging.Messages;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfileImage;

public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  private readonly IEventBus _eventBus;

  public UpdateProfileImageCommandHandler(IUser user, IIdentityService identityService, IUsersRepository usersRepository, IEventBus eventBus)
  {
    _user = user;
    _identityService = identityService;
    _eventBus = eventBus;
    _usersRepository = usersRepository;
  }
  public async Task<Result<Updated>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
  {

    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userResult = await _identityService.GetUserByIdAsync(userId);
    if (userResult.IsError) return userResult.Errors;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;


    var ImageData = await MapIFormToFileData.MapToFileData(request.ProfilePic);
    var UpdateProfileImageMessage = new UpdateProfileImageMessage
    {
      UserProfileId = userProfileResult.Value.Id,
      ProfilePicData = ImageData.Data,
      FileName = request.ProfilePic.FileName,
      ContainerName = Containers.ProfileImages
    };

    await _eventBus.PublishAsync<UpdateProfileImageMessage>(UpdateProfileImageMessage, cancellationToken);

    return Result.Updated;

  }
}