using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Messaging.Consumers;

public class UpdateProfileImageConsumer : IConsumer<UpdateProfileImageMessage>
{
  private readonly ILogger<UpdateProfileImageConsumer> _logger;
  private readonly IUsersRepository usersRepository;

  private readonly IStorageClient _storageClient;
  private readonly IUnitOfWork _unitOfWork;

  public UpdateProfileImageConsumer(ILogger<UpdateProfileImageConsumer> logger, IUsersRepository usersRepository, IStorageClient storageClient, IUnitOfWork unitOfWork)
  {
    _logger = logger;
    this.usersRepository = usersRepository;
    this._storageClient = storageClient;
    this._unitOfWork = unitOfWork;
  }
  public async Task Consume(ConsumeContext<UpdateProfileImageMessage> context)
  {
    var message = context.Message;

    // Here you can implement the logic to handle the profile image update, such as saving the image to a storage service and updating the user's profile in the database.

    _logger.LogInformation("Received UpdateProfileImageMessage: {UserProfileId}", message.UserProfileId);


    var UserProfileResult = await usersRepository.GetUserProfileByIdAsync(message.UserProfileId);
    if (UserProfileResult.IsError)
    {
      _logger.LogError("Failed to retrieve user profile with ID {UserProfileId}: {Errors}", message.UserProfileId, UserProfileResult.Errors);
      return;
    }
    var userProfile = UserProfileResult.Value;

    using var stream = new MemoryStream(message.ProfilePicData);

    var previousProfilePicUrl = userProfile.ProfilePictureUrl;



    var uploadResult = await _storageClient.UploadFileAsync(stream, message.FileName, message.ContainerName, default);

    if (uploadResult.IsError)
    {
      _logger.LogError("Failed to upload profile image for user profile ID {UserProfileId}: {Errors}", message.UserProfileId, uploadResult.Errors);

      return;
    }

    if (previousProfilePicUrl is not null)
      await _storageClient.DeleteFileAsync(previousProfilePicUrl, message.ContainerName);


    userProfile.UpdateProfilePicture(uploadResult.Value);

    await _unitOfWork.SaveChangesAsync();



  }
}