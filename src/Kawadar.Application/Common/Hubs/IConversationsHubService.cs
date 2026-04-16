using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations.Messages;

namespace Kawadar.Application.Common.Hubs;

public interface IConversationsHubService
{
  Task<Result<bool>> IsUserInConversationAsync(Guid conversationId, string userId);
  Task SendMessageToConversationAsync(Guid conversationId, string? connectionId, string recipientId, MessageDto message);
}