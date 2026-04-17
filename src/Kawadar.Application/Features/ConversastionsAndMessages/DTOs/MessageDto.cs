namespace Kawadar.Application.Features.ConversastionsAndMessages.DTOs;

public class MessageDto
{
  public Guid Id { get; set; }
  public string Content { get; set; } = null!;
  public string SenderUserName { get; set; } = null!;
  public DateTime SentAt { get; set; }
  public Guid ConversationId { get; set; }
  public MessageReplyDto? messageReplyDto { get; set; }

  public List<MessageAttachmentDto>? Attachments { get; set; }
}

public class MessageReplyDto
{
  public Guid Id { get; set; }
  public string Content { get; set; } = null!;


}
public class MessageAttachmentDto
{
  public string FileName { get; set; } = null!;
  public string FileUrl { get; set; } = null!;
  public string ContentType { get; set; } = null!;
  public long FileSizeInBytes { get; set; }
  public Guid Id { get; set; }
}