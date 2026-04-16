using Kawadar.Domain.Common;
using Kawadar.Domain.Conversations.Messages;

namespace Kawadar.Domain.Conversations.Events;

public class CreatedMessageEvent : DomainEvent
{
  public Message Message { get; }
  public Guid ConversationId { get; }
  public string RecipientUserId { get; }

  public Guid RecipientUserProfileId { get; set; }
  public string? ConnectionId { get; set; }

  public CreatedMessageEvent(Message message, Guid conversationId, string recipientUserId, Guid recipientUserProfileId, string? connectionId)
  {
    Message = message;
    ConversationId = conversationId;
    RecipientUserId = recipientUserId;
    RecipientUserProfileId = recipientUserProfileId;
    ConnectionId = connectionId;


  }
}