using Kawadar.Application.Common.Models;

namespace Kawadar.Application.Features.ConversastionsAndMessages.DTOs;

public class ConversationMessagesDto
{
  public Guid? ProposalId { get; set; }
  public Guid? JobId { get; set; }
  public PaginatedList<MessageDto> Messages { get; set; } = null!;
}
