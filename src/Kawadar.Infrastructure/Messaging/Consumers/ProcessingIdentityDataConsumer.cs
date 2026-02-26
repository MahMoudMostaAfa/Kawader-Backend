using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Messaging.Consumers;

public class ProcessingIdentityDataConsumer : IConsumer<ProcessingIdentityDataMessage>
{
  private readonly ILogger<ProcessingIdentityDataConsumer> _logger;

  private readonly IUsersRepository _usersRepository;
  private readonly IAIService _aIService;

  private readonly IUnitOfWork _unitOfWork;

  public ProcessingIdentityDataConsumer(ILogger<ProcessingIdentityDataConsumer> logger, IUsersRepository usersRepository, IAIService aIService, IUnitOfWork unitOfWork)
  {
    _logger = logger;
    _usersRepository = usersRepository;
    _aIService = aIService;
    _unitOfWork = unitOfWork;
  }
  public async Task Consume(ConsumeContext<ProcessingIdentityDataMessage> context)
  {
    var message = context.Message;

    _logger.LogInformation("Received ProcessingIdentityDataMessage: {UserProfileId}", message.UserProfileId);
    var UserProfileResult = await _usersRepository.GetUserProfileByIdAsync(message.UserProfileId);
    if (UserProfileResult.IsError)
    {
      _logger.LogError("Failed to retrieve user profile with ID {UserProfileId}: {Errors}", message.UserProfileId, UserProfileResult.Errors);

      throw new Exception($"Failed to retrieve user profile with ID {message.UserProfileId}: {UserProfileResult.Errors}");
    }
    var userProfile = UserProfileResult.Value;
    var AiResult = await _aIService.GenerateStructuredResponseAsync<ResponseDto>("extract identity data , birthdate in this xxxx-02-30  format ", new List<FileData>
    {
      new FileData(message.IdentityFrontPicData, "image/jpeg"),
    }, default);

    if (AiResult.IsError)
    {
      _logger.LogError("Failed to process identity data for user profile ID {UserProfileId}: {Errors}", message.UserProfileId, AiResult.Errors);

      throw new Exception($"Failed to process identity data for user profile ID {message.UserProfileId}: {AiResult.Errors}");
    }

    var response = AiResult.Value;

    Console.WriteLine($"AI Response for user profile ID {message.UserProfileId}: IsTheIdentityValid={response.IsTheIdentityValid}, IdentityName={response.IdentityName}, IdentityNumber={response.IdentityNumber}, IdentityLocation={response.IdentityLocation}, DateOfBirth={response.DateOfBirth}");
    if (response.IsTheIdentityValid)
    {
      userProfile.UpdateIdentityInfo(response.IdentityNumber, DateOnly.Parse(response.DateOfBirth), response.IdentityLocation, response.IdentityName);
    }
    else
    {
      _logger.LogWarning("The identity data for user profile ID {UserProfileId} is invalid according to AI analysis.", message.UserProfileId);
      throw new Exception($"The identity data for user profile ID {message.UserProfileId} is invalid according to AI analysis.");
    }

    await _unitOfWork.SaveChangesAsync();
    _logger.LogInformation("Finished processing identity data for user profile ID {UserProfileId}", message.UserProfileId);
  }

  public record ResponseDto(bool IsTheIdentityValid, string IdentityName, string IdentityNumber, string IdentityLocation, string DateOfBirth);
}