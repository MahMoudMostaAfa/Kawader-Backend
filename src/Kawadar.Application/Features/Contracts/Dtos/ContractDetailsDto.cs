using Kawadar.Domain.Contracts.Enums;

namespace Kawadar.Application.Features.Contracts.Dtos;

public class ContractDetailsDto
{
  public Guid Id { get; set; }
  public Guid JobId { get; set; }
  public Guid ProposalId { get; set; }
  public Guid OtherPartyId { get; set; }
  public string OtherPartyName { get; set; } = string.Empty;
  public string OtherPartyProfilePictureUrl { get; set; } = string.Empty;
  public string OtherPartyUsername { get; set; } = string.Empty;
  public ContractType ContractType { get; set; }
  public decimal? OneTimeFixedPrice { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public ContractStatus Status { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ContractRole Role { get; set; }
  public int? TotalMilestones { get; set; }
  public List<ContractMilestoneDto>? Milestones { get; set; } = new List<ContractMilestoneDto>();


}

public class ContractMilestoneDto
{
  public Guid Id { get; set; }
  public Guid ProposalMilestoneId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public DateTime DueDate { get; set; }

  public DateTime? CompletionRequestedAt { get; set; }
  public DateTime? CompletionApprovedAt { get; set; }
  public string? RejectionReason { get; set; }

  public int Order { get; set; }

  public ContractMilestoneStatus Status { get; set; }


}