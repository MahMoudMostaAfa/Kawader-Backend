using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
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

  public Result<Deleted> DeleteConversationAsync(Conversation conversation, CancellationToken cancellationToken = default)
  {
    _context.Conversations.Remove(conversation);

    return Result.Deleted;
  }

  public async Task<Result<Conversation>> GetConversationByIdAsync(Guid conversationId, CancellationToken cancellationToken)
  {
    var conversation = await _context.Conversations
      .Include(c => c.LastMessage)
      .Include(c => c.Proposal)
      .FirstOrDefaultAsync(c => c.Id == conversationId);

    if (conversation == null) return Error.NotFound("Conversations.NotFound", "Conversation not found");
    return conversation;
  }

  public async Task<Result<PaginatedList<Conversation>>> GetConversationsForUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
  {
    var conversationsQuery = _context.Conversations
        .Where(c => c.SenderUserId == userId || c.ReceiverUserId == userId)
        .Include(c => c.LastMessage)
      .Include(c => c.Proposal)
        .OrderByDescending(c => c.LastMessage != null ? c.LastMessage.CreatedAt : c.CreatedAt).AsQueryable();

    var totalCount = await conversationsQuery.CountAsync();
    var conversations = await conversationsQuery
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var paginatedList = new PaginatedList<Conversation>(conversations, totalCount, pageNumber, pageSize);

    return paginatedList;

  }

  public async Task<Result<Message>> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
  {
    var message = await _context.Messages.Include(m => m.Files).Include(m => m.ReplayToMessage).FirstOrDefaultAsync(m => m.Id == messageId);
    if (message == null) return Error.NotFound("Messages.NotFound", "Message not found");
    return message;
  }

  public async Task<Result<bool>> ConversationExistsForProposalAsync(Guid proposalId, CancellationToken cancellationToken = default)
  {
    var exists = await _context.Conversations.AnyAsync(c => c.ProposalId == proposalId, cancellationToken);
    return exists;
  }

  public async Task<Result<PaginatedList<Message>>> GetMessagesForConversationAsync(Guid conversationId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
  {
    var query = _context.Messages.Where(m => m.ConversationId == conversationId).Include(m => m.ReplayToMessage).Include(m => m.Files).OrderByDescending(m => m.CreatedAt);
    var totalCount = await query.CountAsync();

    var messages = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    var paginatedList = new PaginatedList<Message>(messages, totalCount, pageNumber, pageSize);

    return paginatedList;
  }

  public async Task<Result<bool>> IsOhterUserJoinedConversationAsync(Guid conversationId, Guid currentUserId, CancellationToken cancellationToken = default)
  {
    var isOtherUserJoined = await _context.Conversations.Where(c => c.Id == conversationId).AnyAsync(c => c.Messages.Any(m => m.SenderUserId != currentUserId), cancellationToken);

    return isOtherUserJoined;
  }


}