using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations.Events;
using Kawadar.Domain.Conversations.Messages;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.Notifications.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;


public class CreatedMessageEventHandler : INotificationHandler<CreatedMessageEvent>
{
  private readonly ILogger<CreatedMessageEventHandler> _logger;
  private readonly IConversationsHubService _conversationsHubService;
  private readonly INotificationsHubService _notificationsHubService;
  private readonly INotificationsRepository _notificationsRepository;
  private readonly IConversationsRepository _conversationsRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IPersistanceService _persistanceService;
  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  public CreatedMessageEventHandler(ILogger<CreatedMessageEventHandler> logger, IConversationsHubService conversationsHubService, INotificationsHubService notificationsHubService, IPersistanceService persistanceService, INotificationsRepository notificationsRepository, IUnitOfWork unitOfWork, IConversationsRepository conversationsRepository, IIdentityService identityService, IUsersRepository usersRepository)
  {
    _logger = logger;
    _conversationsHubService = conversationsHubService;
    _notificationsHubService = notificationsHubService;
    _persistanceService = persistanceService;
    _notificationsRepository = notificationsRepository;
    _unitOfWork = unitOfWork;
    _conversationsRepository = conversationsRepository;
    _identityService = identityService;
    _usersRepository = usersRepository;

  }
  public async Task Handle(CreatedMessageEvent notification, CancellationToken cancellationToken)
  {

    // get replay message if exists
    MessageReplyDto? replayMessageDto = null;

    if (notification.Message.ReplayToMessageId != null)
    {
      var replayMessageResult = await _conversationsRepository.GetMessageByIdAsync(notification.Message.ReplayToMessageId.Value);
      if (replayMessageResult.IsError)
      {
        _logger.LogError("Failed to retrieve replay message with ID {replayToMessageId}: {errors}", notification.Message.ReplayToMessageId.Value, replayMessageResult.Errors);
      }
      else
      {
        var replayMessage = replayMessageResult.Value;
        replayMessageDto = new MessageReplyDto
        {
          Id = replayMessage.Id,
          Content = replayMessage.Content,
        };
      }
    }

    var senderUserResult = await _usersRepository.GetUserProfileByIdAsync(notification.Message.SenderUserId);
    if (senderUserResult.IsError)
    {
      _logger.LogError("Failed to retrieve sender user profile with ID {senderUserId}: {errors}", notification.Message.SenderUserId, senderUserResult.Errors);
    }
    var senderUser = senderUserResult.Value;
    var identityResult = await _identityService.GetUserByIdAsync(senderUser.UserId);
    if (identityResult.IsError)
    {
      _logger.LogError("Failed to retrieve sender user identity with ID {senderUserId}: {errors}", senderUser.UserId, identityResult.Errors);
    }
    var senderIdentity = identityResult.Value;

    MessageDto message = new MessageDto
    {
      Id = notification.Message.Id,
      Content = notification.Message.Content,
      SenderUserName = senderIdentity.UserName,
      SentAt = notification.Message.CreatedAt,
      ConversationId = notification.Message.ConversationId,
      messageReplyDto = replayMessageDto,
      Attachments = notification.Message.Files.Select(a => new MessageAttachmentDto
      {
        Id = a.Id,
        ContentType = a.File.MimeType,
        FileName = a.File.FileName,
        FileSizeInBytes = a.File.FileSizeInBytes,
        FileUrl = a.File.FileUrl,
      }).ToList()
    };


    // Send real-time message to conversation participants via SignalR
    await _conversationsHubService.SendMessageToConversationAsync(notification.ConversationId, notification.ConnectionId, notification.RecipientUserId, message);

    // check if recipient is online and send notification via SignalR
    var isRecipientOnline = await _persistanceService.IsUserOnlineAsync(notification.RecipientUserId);
    if (isRecipientOnline)
    {
      // create and send real-time notification via SignalR
      var notificationResult = Notification.Create(notification.RecipientUserProfileId, "New message received", "you have received a new message in one of your conversations ", NotificationCategory.Message, NotificationType.Info, notification.ConversationId, "conversations", $"conversations/{notification.ConversationId}");
      if (notificationResult.IsError)
      {
        _logger.LogError("Failed to create notification for new message: {errors}", notificationResult.Errors);
        return;
      }
      var newNotification = notificationResult.Value;
      await _notificationsRepository.AddNotificationAsync(newNotification);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      // send real-time notification via SignalR
      await _notificationsHubService.SendNotificationAsync(notification.RecipientUserId, new NotificationDto
      {
        Id = newNotification.Id,
        Title = newNotification.Title,
        Body = newNotification.Body,
        Category = newNotification.Category.ToString(),
        Type = newNotification.Type.ToString(),
        ReceivedAt = newNotification.CreatedAt,
        IsRead = newNotification.IsRead,
        RedirectUrl = newNotification.RedirectUrl,
      });

    }
    else
    {
      // TODO: SEND MAIL Notification  to its email 
      _logger.LogInformation("Recipient user {recipientUserId} is offline. Consider sending an email or push notification.", notification.RecipientUserId);
    }
  }
}