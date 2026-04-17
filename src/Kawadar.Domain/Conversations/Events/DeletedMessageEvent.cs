using Kawadar.Domain.Common;

namespace Kawadar.Domain.Conversations.Events;

public class DeletedMessageEvent : DomainEvent
{
  public Guid MessageId { get; set; }

  public Guid ConversationId { get; set; }

  public string userId { get; set; } = null!;

  public string ConnectionId { get; set; } = null!;
  public DateTime SentAt { get; set; }

  public Guid UserProfileId { get; set; }

  public string NewContent { get; set; } = null!;

  public DeletedMessageEvent(Guid messageId, Guid conversationId, string userId, string connectionId, DateTime sentAt, Guid userProfileId, string newContent)
  {
    MessageId = messageId;
    ConversationId = conversationId;
    this.userId = userId;
    ConnectionId = connectionId;
    SentAt = sentAt;
    UserProfileId = userProfileId;
    NewContent = newContent;


  }
}