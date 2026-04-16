using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations;
using Kawadar.Domain.Conversations.Messages;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

  public async Task<Result<Conversation>> GetConversationByIdAsync(Guid conversationId, CancellationToken cancellationToken)
  {
    var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId);

    if (conversation == null) return Error.NotFound("Conversations.NotFound", "Conversation not found");
    return conversation;
  }

  public async Task<Result<Message>> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
  {
    var message = await _context.Messages.Include(m => m.Files).FirstOrDefaultAsync(m => m.Id == messageId);
    if (message == null) return Error.NotFound("Messages.NotFound", "Message not found");
    return message;
  }
}