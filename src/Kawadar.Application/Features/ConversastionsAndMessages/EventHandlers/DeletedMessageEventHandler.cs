using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Conversations.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;

public class DeletedMessageEventHandler : INotificationHandler<DeletedMessageEvent>
{
  private readonly IConversationsHubService _conversationsHubService;
  private readonly ILogger<DeletedMessageEventHandler> _logger;
  public DeletedMessageEventHandler(IConversationsHubService conversationsHubService, ILogger<DeletedMessageEventHandler> logger)
  {
    _conversationsHubService = conversationsHubService;
    _logger = logger;

  }
  public async Task Handle(DeletedMessageEvent notification, CancellationToken cancellationToken)
  {
    var messageDto = new MessageDto
    {
      Id = notification.MessageId,
      Content = notification.NewContent,
      SenderId = notification.UserProfileId,
      SentAt = notification.SentAt,
      ConversationId = notification.ConversationId,
    };
    await _conversationsHubService.SendDeletedMessageToConversationAsync(notification.ConversationId, notification.ConnectionId, notification.userId, messageDto);

    _logger.LogInformation("DeletedMessageEvent handled for MessageId: {MessageId}, ConversationId: {ConversationId}, UserId: {UserId}", notification.MessageId, notification.ConversationId, notification.userId);
  }
}