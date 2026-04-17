namespace Kawadar.Infrastructure.Services.HubServices.SignalRDTOs;

public class EditSignalRMessageRequest
{
  public Guid ConversationId { get; set; }
  public Guid MessageId { get; set; }
  public string NewContent { get; set; } = null!;
}