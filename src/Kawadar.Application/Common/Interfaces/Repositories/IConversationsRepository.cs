using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations;
using Kawadar.Domain.Conversations.Messages;

namespace Kawadar.Application.Common.Interfaces.Repositories;


public interface IConversationsRepository
{
  Task<Result<Created>> AddConversationAsync(Conversation conversation, CancellationToken cancellationToken = default);
  Task<Result<Created>> AddMessageAsync(Message message, CancellationToken cancellationToken = default);

  Task<Result<Conversation>> GetConversationByIdAsync(Guid conversationId, CancellationToken cancellationToken = default);

  Task<Result<PaginatedList<Conversation>>> GetConversationsForUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
  Task<Result<Message>> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default);

  Result<Deleted> DeleteConversationAsync(Conversation conversation, CancellationToken cancellationToken = default);
  Task<Result<bool>> IsOhterUserJoinedConversationAsync(Guid conversationId, Guid currentUserId, CancellationToken cancellationToken = default);
  Task<Result<PaginatedList<Message>>> GetMessagesForConversationAsync(Guid conversationId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}