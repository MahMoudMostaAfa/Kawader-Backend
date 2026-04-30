using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Enums;

namespace Kawadar.Domain.Contracts;

public class ContractMilestone : AuditableEntity
{
  public Guid ContractId { get; private set; }
  public Guid ProposalMilestoneId { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public string Description { get; private set; } = string.Empty;
  public decimal Amount { get; private set; } = 0;
  public int Order { get; private set; } = 0;
  public ContractMilestoneStatus Status { get; private set; } = ContractMilestoneStatus.Pending;

  public DateTime DueDate { get; private set; }


  public DateTime? CompletionRequestedAt { get; private set; }

  public DateTime? CompletionApprovedAt { get; private set; }

  private ContractMilestone()
  {

  }

  private ContractMilestone(Guid contractId, Guid proposalMilestoneId, string title, string description, decimal amount, int order, DateTime dueDate) : base(Guid.NewGuid())
  {
    ContractId = contractId;
    ProposalMilestoneId = proposalMilestoneId;
    Title = title;
    Description = description;
    Amount = amount;
    Order = order;
    DueDate = dueDate;
  }

  public static Result<ContractMilestone> Create(Guid contractId, Guid proposalMilestoneId, string title, string description, decimal amount, int order, DateTime dueDate)
  {
    return new ContractMilestone(contractId, proposalMilestoneId, title, description, amount, order, dueDate);
  }



}