using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UploadIdentity;


public class UploadIdentityCommandHandler : IRequestHandler<UploadIdentityCommand, Result<Success>>
{
  private readonly IStorageClient _storageClient;
  private readonly IIdentityService _identityService;
  private readonly IUser _user;

  private readonly IUsersRepository _usersRepository;

  private readonly IUnitOfWork _unitOfWork;

  public UploadIdentityCommandHandler(IStorageClient storageClient, IIdentityService identityService, IUser user, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
  {
    _storageClient = storageClient;
    _identityService = identityService;
    _user = user;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
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

    // upload front and back images in parallel
    using var stream = request.FrontImage.OpenReadStream();
    using var backStream = request.BackImage.OpenReadStream();

    var frontTask = _storageClient.UploadFileAsync(stream, request.FrontImage.FileName, Containers.IdentityImages, cancellationToken);
    var backTask = _storageClient.UploadFileAsync(backStream, request.BackImage.FileName, Containers.IdentityImages, cancellationToken);

    await Task.WhenAll(frontTask, backTask);

    var frontImageUrlResult = frontTask.Result;
    var backImageUrlResult = backTask.Result;

    if (frontImageUrlResult.IsError) return frontImageUrlResult.Errors;
    if (backImageUrlResult.IsError) return backImageUrlResult.Errors;


    var UpdateIdentityImagesResult = userProfile.UpdateIdentityImages(frontImageUrlResult.Value, backImageUrlResult.Value);
    if (UpdateIdentityImagesResult.IsError) return UpdateIdentityImagesResult.Errors;

    return await _unitOfWork.SaveChangesAsync() > 0
        ? Result.Success
        : ApplicationErrors.FailedToUploadIdentity;
  }
}