namespace Kawadar.Api.Requests.Conversation;


public class CreateConversationRequest
{
  public string ReceiverUserName { get; set; } = default!;
  public Guid ProposalId { get; set; }
  public string Title { get; set; } = default!;
  public string InitialMessageContent { get; set; } = default!;
}