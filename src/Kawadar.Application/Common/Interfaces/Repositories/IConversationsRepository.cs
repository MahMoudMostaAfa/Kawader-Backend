using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations;
using Kawadar.Domain.Conversations.Messages;

namespace Kawadar.Application.Common.Interfaces.Repositories;


public interface IConversationsRepository
{
  Task<Result<Created>> AddConversationAsync(Conversation conversation, CancellationToken cancellationToken);
  Task<Result<Created>> AddMessageAsync(Message message, CancellationToken cancellationToken);
}