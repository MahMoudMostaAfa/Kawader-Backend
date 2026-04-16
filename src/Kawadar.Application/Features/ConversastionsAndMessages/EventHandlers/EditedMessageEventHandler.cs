using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Conversations.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;

public class EditedMessageEventHandler : INotificationHandler<EditedMessageEvent>
{
  private readonly IConversationsHubService _conversationsHubService;
  private readonly ILogger<EditedMessageEventHandler> _logger;
  public EditedMessageEventHandler(IConversationsHubService conversationsHubService, ILogger<EditedMessageEventHandler> logger)
  {
    _conversationsHubService = conversationsHubService;
    _logger = logger;

  }
  public async Task Handle(EditedMessageEvent notification, CancellationToken cancellationToken)
  {
    var messageDto = new MessageDto
    {
      Id = notification.MessageId,
      Content = notification.NewContent,
      SenderId = notification.UserProfileId,
      SentAt = notification.SentAt,
      ConversationId = notification.ConversationId,
    };
    await _conversationsHubService.SendEditedMessageToConversationAsync(notification.ConversationId, notification.ConnectionId, notification.userId, messageDto);

    _logger.LogInformation("EditedMessageEvent handled for MessageId: {MessageId}, ConversationId: {ConversationId}, UserId: {UserId}", notification.MessageId, notification.ConversationId, notification.userId);


  }
}