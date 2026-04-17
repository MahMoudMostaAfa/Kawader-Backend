namespace Kawadar.Api.Requests.Conversation;

public class SendMessageRequest
{

  public string Content { get; set; } = null!;
  public Guid? ReplyToMessageId { get; set; }

  public string ConnectionId { get; set; } = null!;
  public List<IFormFile>? AttachmentFiles { get; set; }
  public List<string>? AttachmentLinks { get; set; }


}