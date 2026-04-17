using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Conversations.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;

public class DeletedMessageEventHandler : INotificationHandler<DeletedMessageEvent>
{
  private readonly IConversationsHubService _conversationsHubService;
  private readonly ILogger<DeletedMessageEventHandler> _logger;
  private readonly IIdentityService _identityService;
  private readonly IUsersRepository _usersRepository;
  public DeletedMessageEventHandler(IConversationsHubService conversationsHubService, ILogger<DeletedMessageEventHandler> logger, IIdentityService identityService, IUsersRepository usersRepository)
  {
    _conversationsHubService = conversationsHubService;
    _logger = logger;
    _identityService = identityService;
    _usersRepository = usersRepository;

  }
  public async Task Handle(DeletedMessageEvent notification, CancellationToken cancellationToken)
  {

    var userResult = await _usersRepository.GetUserProfileByIdAsync(notification.UserProfileId);
    if (userResult.IsError)
    {
      _logger.LogError("Failed to retrieve user profile with ID {UserProfileId}: {errors}", notification.UserProfileId, userResult.Errors);
      return;
    }
    var userProfile = userResult.Value;

    var identityResult = await _identityService.GetUserByIdAsync(userProfile.UserId);
    if (identityResult.IsError)
    {
      _logger.LogError("Failed to retrieve identity for user with ID {UserId}: {errors}", userProfile.UserId, identityResult.Errors);
      return;
    }
    var identityUser = identityResult.Value;
    var messageDto = new MessageDto
    {
      Id = notification.MessageId,
      Content = notification.NewContent,
      SenderUserName = identityUser.UserName,
      SentAt = notification.SentAt,
      ConversationId = notification.ConversationId,
    };
    await _conversationsHubService.SendDeletedMessageToConversationAsync(notification.ConversationId, notification.ConnectionId, notification.userId, messageDto);

    _logger.LogInformation("DeletedMessageEvent handled for MessageId: {MessageId}, ConversationId: {ConversationId}, UserId: {UserId}", notification.MessageId, notification.ConversationId, notification.userId);
  }
}