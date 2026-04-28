using Kawadar.Domain.Contracts.Enums;

namespace Kawadar.Application.Features.Contracts.Dtos;

public class ContractDto
{
  public Guid Id { get; set; }
  public Guid JobId { get; set; }
  public Guid ProposalId { get; set; }
  public Guid OtherPartyId { get; set; }
  public ContractType ContractType { get; set; }
  public decimal? OneTimeFixedPrice { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public ContractStatus Status { get; set; }
  public int? TotalMilestones { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ContractRole Role { get; set; }

}

public enum ContractRole
{
  Client = 1,
  Freelancer = 2
}