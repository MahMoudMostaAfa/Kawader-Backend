using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Conversations.Messages;

public class Message : AuditableEntity
{

  public Guid ConversationId { get; private set; }
  public Guid SenderUserId { get; private set; }
  public Guid? ReplayToMessageId { get; private set; }
  public string Content { get; private set; } = default!;

  public bool IsDeleted { get; private set; } = false;

  private readonly List<MessageFile> _files = [];

  public IReadOnlyList<MessageFile> Files => _files.AsReadOnly();

  private Message() { }

  private Message(Guid conversationId, Guid senderUserId, string content, Guid? replayToMessageId, List<MessageFile>? files = null) : base(Guid.NewGuid())
  {
    ConversationId = conversationId;
    SenderUserId = senderUserId;
    Content = content;
    ReplayToMessageId = replayToMessageId;

    if (files is not null)
    {
      _files.AddRange(files);
    }
  }


  public static Result<Message> Create(Guid conversationId, Guid senderUserId, string content, Guid? replayToMessageId, List<MessageFile>? files = null)
  {
    return new Message(conversationId, senderUserId, content, replayToMessageId, files);
  }

  public Result<Updated> UpdateContent(string newContent)
  {
    Content = newContent;
    return Result.Updated;
  }

  public Result<Deleted> Delete()
  {
    IsDeleted = true;
    Content = "[Deleted]";
    _files.Clear(); // Optionally, you can also clear the attached files when a message is deleted.

    return Result.Deleted;
  }



}