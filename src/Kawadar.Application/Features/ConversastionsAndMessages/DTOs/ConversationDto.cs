namespace Kawadar.Application.Features.ConversastionsAndMessages.DTOs;

public class ConversationDto
{
  public Guid Id { get; set; }
  public Guid? ProposalId { get; set; }
  public Guid? JobId { get; set; }
  public string Title { get; set; } = null!;
  public string LastMessageContent { get; set; } = null!;
  public bool IsLastMessageByCurrentUser { get; set; }
  public string OtherParticipantUserName { get; set; } = null!;
  public string OtherParticipantFullName { get; set; } = null!;
  public string otherParticipantProfilePictureUrl { get; set; } = null!;
  public DateTime LastMessageSentAt { get; set; }

}