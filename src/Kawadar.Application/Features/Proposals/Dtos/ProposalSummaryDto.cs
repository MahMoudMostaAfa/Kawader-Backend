using Kawadar.Domain.Proposals.Enums;

namespace Kawadar.Application.Features.Proposals.Dtos;

public class ProposalSummaryDto
{
  public Guid Id { get; set; }
  public string? FreelancerName { get; set; } = string.Empty;
  public string? FreelancerProfilePictureUrl { get; set; } = string.Empty;

  public string? FreelancerUsername { get; set; } = string.Empty;

  public string CoverLetter { get; set; } = string.Empty;

  public JobProposalType ProposalType { get; set; }

  public JobProposalStatus Status { get; set; }
  public decimal ProposedPrice { get; set; }
  public int? EstimatedTimeInDays { get; set; }
  public int? EstimatedTimeInHours { get; set; }
  public int? TotalMilestones { get; set; }
  public DateTime CreatedAt { get; set; }
}