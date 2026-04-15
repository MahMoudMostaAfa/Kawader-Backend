using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Conversations.Messages;

public class MessageFile : AuditableEntity
{


  public Common.ValueObjects.FileInfo File { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private MessageFile() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private MessageFile(Common.ValueObjects.FileInfo file) : base(Guid.NewGuid())
  {
    File = file;

  }

  public static Result<MessageFile> Create(Common.ValueObjects.FileInfo file)
  {
    return new MessageFile(file);
  }
}