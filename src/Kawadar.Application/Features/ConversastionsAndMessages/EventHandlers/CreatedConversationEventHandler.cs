using AutoMapper;
using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Conversations.Events;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.Notifications.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;

public class CreatedConversationEventHandler : INotificationHandler<CreatedConversationEvent>
{

  private readonly ILogger<CreatedConversationEventHandler> _logger;

  private readonly IUnitOfWork _unitOfWork;
  private readonly IUsersRepository _usersRepository;
  private readonly IJobsRepository _jobsRepository;

  private readonly INotificationsRepository _notificationRepository;
  private readonly INotificationsHubService _notificationsHubService;
  private readonly IMapper _mapper;
  public CreatedConversationEventHandler(ILogger<CreatedConversationEventHandler> logger, IUnitOfWork unitOfWork, IUsersRepository usersRepository, IJobsRepository jobsRepository, INotificationsRepository notificationRepository, INotificationsHubService notificationsHubService, IMapper mapper)
  {
    _unitOfWork = unitOfWork;
    _usersRepository = usersRepository;
    _jobsRepository = jobsRepository;
    _notificationRepository = notificationRepository;
    _notificationsHubService = notificationsHubService;
    _mapper = mapper;

    _logger = logger;
  }
  public async Task Handle(CreatedConversationEvent notification, CancellationToken cancellationToken)
  {
    var recieverUserProfileResult = await _usersRepository.GetUserProfileByIdAsync(notification.ReceiverUserId);
    if (recieverUserProfileResult.IsError)
    {
      _logger.LogError("Failed to retrieve receiver user profile for CreatedConversationEvent. UserId: {UserId}, Errors: {Errors}", notification.ReceiverUserId, recieverUserProfileResult.Errors);
      return;
    }
    var recieverUserProfile = recieverUserProfileResult.Value;

    Domain.Jobs.Job? job = null;
    if (notification.JobId is not null)
    {
      var jobResult = await _jobsRepository.GetJobByIdAsync(notification.JobId.Value);
      if (jobResult.IsError)
      {
        _logger.LogError("Failed to retrieve job for CreatedConversationEvent. JobId: {JobId}, Errors: {Errors}", notification.JobId.Value, jobResult.Errors);
        return;
      }
      job = jobResult.Value;
    }

    var senderUserProfileResult = await _usersRepository.GetUserProfileByIdAsync(notification.SenderUserId);
    if (senderUserProfileResult.IsError)
    {
      _logger.LogError("Failed to retrieve sender user profile for CreatedConversationEvent. UserId: {UserId}, Errors: {Errors}", notification.SenderUserId, senderUserProfileResult.Errors);
      return;
    }
    var senderUserProfile = senderUserProfileResult.Value;
    // create a notification for the receiver about the new conversation  
    var notificationResult = Notification.Create(notification.ReceiverUserId, "New Conversation", $"You have a new conversation from user {senderUserProfile.FullName}" + (job is not null ? $" regarding the job {job.Title}" : ""), NotificationCategory.Message, NotificationType.Info, notification.ConversationId, "Conversations", "/conversations/" + notification.ConversationId);
    if (notificationResult.IsError)
    {
      _logger.LogError("Failed to create notification for CreatedConversationEvent. Errors: {Errors}", notificationResult.Errors);
      return;
    }
    var notificationEntity = notificationResult.Value;

    var saveResult = await _notificationRepository.AddNotificationAsync(notificationEntity, cancellationToken);
    if (saveResult.IsError)
    {
      _logger.LogError("Failed to save notification for CreatedConversationEvent. Errors: {Errors}", saveResult.Errors);
      return;
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Created notification for new conversation. ConversationId: {ConversationId}, ReceiverUserId: {ReceiverUserId}", notification.ConversationId, notification.ReceiverUserId);
    var notificationDto = _mapper.Map<NotificationDto>(notificationEntity);

    await _notificationsHubService.SendNotificationAsync(recieverUserProfile.UserId, notificationDto, cancellationToken);


  }
}