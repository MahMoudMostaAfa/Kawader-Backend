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

  public string? RejectionReason { get; private set; }

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

  public Result<Updated> UpdateDetails(string title, string description, decimal amount, DateTime dueDate)
  {
    Title = title;
    Description = description;
    Amount = amount;
    DueDate = dueDate;
    return Result.Updated;
  }

  public Result<Updated> UpdateDueDate(DateTime dueDate)
  {
    DueDate = dueDate;
    return Result.Updated;
  }

  public void UpdateOrder(int order)
  {
    Order = order;
  }

  public Result<Updated> Start()
  {
    if (Status != ContractMilestoneStatus.Pending)
      return Error.Conflict("Contracts.Milestones", "Only pending milestones can be started.");

    Status = ContractMilestoneStatus.InProgress;
    return Result.Updated;
  }

  public Result<Updated> SubmitForReview()
  {
    if (Status != ContractMilestoneStatus.InProgress)
      return Error.Conflict("Contracts.Milestones", "Only in-progress milestones can be submitted for review.");

    Status = ContractMilestoneStatus.SubmittedForReview;
    CompletionRequestedAt = DateTime.UtcNow;
    return Result.Updated;
  }

  public Result<Updated> Approve()
  {
    if (Status != ContractMilestoneStatus.SubmittedForReview)
      return Error.Conflict("Contracts.Milestones", "Only milestones submitted for review can be approved.");

    Status = ContractMilestoneStatus.Approved;
    CompletionApprovedAt = DateTime.UtcNow;
    return Result.Updated;
  }

  public Result<Updated> Reject(string? reason)
  {
    if (Status != ContractMilestoneStatus.SubmittedForReview)
      return Error.Conflict("Contracts.Milestones", "Only milestones submitted for review can be rejected.");

    RejectionReason = reason;
    Status = ContractMilestoneStatus.InProgress;
    CompletionRequestedAt = null;
    CompletionApprovedAt = null;
    return Result.Updated;
  }



}