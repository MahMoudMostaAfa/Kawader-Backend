using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations;
using Kawadar.Domain.Conversations.Messages;
using Kawadar.Infrastructure.Data;

namespace Kawadar.Infrastructure.Services.Repositories;

public class ConversationsRepository : IConversationsRepository
{
  private readonly AppDbContext _context;

  public ConversationsRepository(AppDbContext context)
  {
    _context = context;
  }
  public async Task<Result<Created>> AddConversationAsync(Conversation conversation, CancellationToken cancellationToken)
  {
    await _context.Conversations.AddAsync(conversation, cancellationToken);
    return Result.Created;
  }

  public async Task<Result<Created>> AddMessageAsync(Message message, CancellationToken cancellationToken)
  {
    await _context.Messages.AddAsync(message, cancellationToken);
    return Result.Created;
  }
}