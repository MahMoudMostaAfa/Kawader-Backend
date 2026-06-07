using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations.Enums;
using Kawadar.Domain.Conversations.Events;
using Kawadar.Domain.Conversations.Messages;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Proposals;

namespace Kawadar.Domain.Conversations;

public class Conversation : AuditableEntity
{

  public string Title { get; private set; } = default!;

  public ConversationStatus ConversationStatus { get; private set; } = ConversationStatus.Open;

  public Guid SenderUserId { get; private set; }

  public Guid ReceiverUserId { get; private set; }

  public Guid? JobId { get; private set; }
  public Job? Job { get; private set; }
  public Guid? ProposalId { get; private set; }
  public JobProposal? Proposal { get; private set; }
  public Guid? LastMessageId { get; private set; }

  public Message? LastMessage { get; private set; }

  private readonly List<Message> _messages = [];
  public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

  private Conversation() { }


  private Conversation(string title, Guid senderUserId, Guid receiverUserId, Guid proposalId, Guid? jobId) : base(Guid.NewGuid())
  {
    Title = title;
    SenderUserId = senderUserId;
    ReceiverUserId = receiverUserId;
    ProposalId = proposalId;
    JobId = jobId;
  }


  public static Result<Conversation> Create(string title, Guid senderUserId, Guid receiverUserId, Guid proposalId, Guid? jobId)
  {
    if (senderUserId == receiverUserId) return ConversationErrors.SenderAndReceiverCannotBeTheSame;

    var conversation = new Conversation(title, senderUserId, receiverUserId, proposalId, jobId);


    return conversation;

  }

  public Result<Updated> SetLastMessage(Message message)
  {
    if (message.ConversationId != Id) return ConversationErrors.MessageDoesNotBelongToConversation;

    LastMessageId = message.Id;
    LastMessage = message;

    return Result.Updated;
  }

  public Result<Updated> ClearLastMessageReference()
  {
    LastMessageId = null;
    LastMessage = null;
    return Result.Updated;
  }


  public Result<Updated> UpdateConversationStatus(ConversationStatus newStatus)
  {
    ConversationStatus = newStatus;
    return Result.Updated;
  }



}
