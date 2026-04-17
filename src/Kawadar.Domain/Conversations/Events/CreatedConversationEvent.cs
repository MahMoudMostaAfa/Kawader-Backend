using Kawadar.Domain.Common;

namespace Kawadar.Domain.Conversations.Events;


public class CreatedConversationEvent : DomainEvent
{
  public Guid ConversationId { get; }
  public Guid SenderUserId { get; }
  public Guid ReceiverUserId { get; }
  public Guid? JobId { get; }

  public CreatedConversationEvent(Guid conversationId, Guid senderUserId, Guid receiverUserId, Guid? jobId)
  {
    ConversationId = conversationId;
    SenderUserId = senderUserId;
    ReceiverUserId = receiverUserId;
    JobId = jobId;
  }
}