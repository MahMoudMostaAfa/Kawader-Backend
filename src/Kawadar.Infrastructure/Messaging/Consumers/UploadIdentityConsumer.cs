using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Messaging.Consumers;


public class UploadIdentityConsumer : IConsumer<UploadIdentityMessage>
{
  private readonly ILogger<UploadIdentityConsumer> _logger;
  private readonly IUsersRepository usersRepository;
  private readonly IStorageClient _storageClient;
  private readonly IUnitOfWork _unitOfWork;

  public UploadIdentityConsumer(ILogger<UploadIdentityConsumer> logger, IUsersRepository usersRepository, IStorageClient storageClient, IUnitOfWork unitOfWork)
  {
    _logger = logger;
    this.usersRepository = usersRepository;
    this._storageClient = storageClient;
    this._unitOfWork = unitOfWork;

  }
  public async Task Consume(ConsumeContext<UploadIdentityMessage> context)
  {
    var message = context.Message;

    _logger.LogInformation("Received UploadIdentityMessage: {UserProfileId}", message.UserProfileId);


    var UserProfileResult = await usersRepository.GetUserProfileByIdAsync(message.UserProfileId);
    if (UserProfileResult.IsError)
    {
      _logger.LogError("Failed to retrieve user profile with ID {UserProfileId}: {Errors}", message.UserProfileId, UserProfileResult.Errors);

      throw new Exception($"Failed to retrieve user profile with ID {message.UserProfileId}: {UserProfileResult.Errors}");
    }

    var userProfile = UserProfileResult.Value;

    var frontImageStream = new MemoryStream(message.IdentityFrontPicData);
    var backImageStream = new MemoryStream(message.IdentityBackPicData);

    var frontImageUpload = _storageClient.UploadFileAsync(frontImageStream, message.FileName, message.ContainerName, default);
    var backImageUpload = _storageClient.UploadFileAsync(backImageStream, message.FileName, message.ContainerName, default);

    await Task.WhenAll(frontImageUpload, backImageUpload);

    if (frontImageUpload.Result.IsError || backImageUpload.Result.IsError)
    {
      _logger.LogError("Failed to upload identity images for user profile ID {UserProfileId}: {Errors}", message.UserProfileId, frontImageUpload.Exception ?? backImageUpload.Exception);

      throw new Exception($"Failed to upload identity images for user profile ID {message.UserProfileId}: {frontImageUpload.Exception ?? backImageUpload.Exception}");
    }

    userProfile.UpdateIdentityImages(frontImageUpload.Result.Value, backImageUpload.Result.Value);


    await _unitOfWork.SaveChangesAsync();




  }
}