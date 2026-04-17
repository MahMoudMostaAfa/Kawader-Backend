using Microsoft.AspNetCore.Http;

namespace Kawadar.Infrastructure.Services.HubServices.SignalRDTOs;

public class SendSignalRMessageRequest
{
  public Guid ConversationId { get; set; }
  public string Content { get; set; } = null!;
  public Guid? ReplyToMessageId { get; set; }



}