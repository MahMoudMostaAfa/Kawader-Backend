using System.Text.Json.Nodes;
using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Enums;

namespace Kawadar.Domain.Contracts;

public class Contract : AuditableEntity
{

  public Guid JobId { get; private set; }

  public Guid ProposalId { get; private set; }

  public Guid ClientId { get; private set; }
  public Guid FreelancerId { get; private set; }

  public ContractType Type { get; private set; }


  // for only one-time contract 
  public decimal? OneTimeFixedPrice { get; private set; }

  public ContractStatus Status { get; private set; } = ContractStatus.Active;

  public DateTime? CompletionRequestedAt { get; private set; }
  public DateTime? CompletionApprovedAt { get; private set; }

  public string Title { get; private set; } = string.Empty;
  public string Description { get; private set; } = string.Empty;

  // reson for rejecting completion request, if any
  public string? RejectionReason { get; private set; }


  public DateTime StartAt { get; private set; }
  public DateTime? EndAt { get; private set; }

  private readonly List<ContractMilestone> _contractMilestones = [];

  public IReadOnlyList<ContractMilestone> ContractMilestones => _contractMilestones.AsReadOnly();

  private Contract()
  {

  }

  private Contract(Guid jobId, Guid proposalId, Guid clientId, Guid freelancerId, ContractType type, DateTime startAt, DateTime? endAt, decimal? oneTimeFixedPrice = null, string title = "", string description = "")
  {
    JobId = jobId;
    ProposalId = proposalId;
    ClientId = clientId;
    FreelancerId = freelancerId;
    Type = type;
    StartAt = startAt;
    EndAt = endAt;
    OneTimeFixedPrice = oneTimeFixedPrice;
    Title = title;
    Description = description;

  }

  public static Result<Contract> Create(Guid jobId, Guid proposalId, Guid clientId, Guid freelancerId, ContractType type, DateTime startAt, DateTime? endAt, decimal? oneTimeFixedPrice = null, string title = "", string description = "")
  {
    return new Contract(jobId, proposalId, clientId, freelancerId, type, startAt, endAt, oneTimeFixedPrice, title, description);
  }

  public void AddContractMilestone(Guid ProposalMilestoneId, string title, string description, decimal amount, DateTime dueDate)
  {
    var order = _contractMilestones.Count + 1;
    var contractMilestoneResult = ContractMilestone.Create(Id, ProposalMilestoneId, title, description, amount, order, dueDate);

    var contractMilestone = contractMilestoneResult.Value;

    _contractMilestones.Add(contractMilestone);


  }

  public Result<Updated> UpdateMilestone(Guid milestoneId, DateTime dueDate)
  {
    var milestone = _contractMilestones.FirstOrDefault(m => m.Id == milestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    if (milestone.Status != ContractMilestoneStatus.Pending)
      return Error.Conflict("Contracts.Milestones", "Only pending milestones can be updated.");

    if (dueDate <= DateTime.UtcNow)
      return Error.Validation("Contracts.Milestones", "Due date must be in the future.");

    if (dueDate <= milestone.DueDate)
      return Error.Validation("Contracts.Milestones", "Due date can only be postponed.");

    var nextMilestone = _contractMilestones
      .Where(m => m.Order > milestone.Order)
      .OrderBy(m => m.Order)
      .FirstOrDefault();

    if (nextMilestone is not null && dueDate > nextMilestone.DueDate)
      return Error.Validation("Contracts.Milestones", "Due date cannot exceed the next milestone due date.");

    return milestone.UpdateDueDate(dueDate);
  }

  public Result<Updated> RemoveMilestone(Guid milestoneId)
  {
    if (Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Milestones", "Only active contracts can remove milestones.");

    var milestone = _contractMilestones.FirstOrDefault(m => m.Id == milestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    if (milestone.Status != ContractMilestoneStatus.Pending)
      return Error.Conflict("Contracts.Milestones", "Only pending milestones can be removed.");

    _contractMilestones.Remove(milestone);

    var orderedMilestones = _contractMilestones.OrderBy(m => m.Order).ToList();
    for (var i = 0; i < orderedMilestones.Count; i++)
    {
      orderedMilestones[i].UpdateOrder(i + 1);
    }

    return Result.Updated;
  }

  public Result<Updated> StartMilestone(Guid milestoneId)
  {
    if (Type != ContractType.MilestoneBased)
      return Error.Conflict("Contracts.Milestones", "Milestones can only be started for milestone-based contracts.");

    if (Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Milestones", "Only active contracts can start milestones.");

    var milestone = _contractMilestones.FirstOrDefault(m => m.Id == milestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    var previousMilestone = _contractMilestones
      .Where(m => m.Order < milestone.Order)
      .OrderByDescending(m => m.Order)
      .FirstOrDefault();

    if (previousMilestone is not null && previousMilestone.Status != ContractMilestoneStatus.Approved)
      return Error.Conflict("Contracts.Milestones", "Previous milestone must be approved before starting this milestone.");

    return milestone.Start();
  }

  public Result<Updated> SubmitMilestone(Guid milestoneId)
  {
    if (Type != ContractType.MilestoneBased)
      return Error.Conflict("Contracts.Milestones", "Milestones can only be submitted for milestone-based contracts.");

    if (Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Milestones", "Only active contracts can submit milestones.");

    var milestone = _contractMilestones.FirstOrDefault(m => m.Id == milestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    return milestone.SubmitForReview();
  }

  public Result<Updated> ApproveMilestone(Guid milestoneId)
  {
    if (Type != ContractType.MilestoneBased)
      return Error.Conflict("Contracts.Milestones", "Milestones can only be approved for milestone-based contracts.");

    if (Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Milestones", "Only active contracts can approve milestones.");

    var milestone = _contractMilestones.FirstOrDefault(m => m.Id == milestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    return milestone.Approve();
  }

  public Result<Updated> RejectMilestone(Guid milestoneId, string? reason)
  {
    if (Type != ContractType.MilestoneBased)
      return Error.Conflict("Contracts.Milestones", "Milestones can only be rejected for milestone-based contracts.");

    if (Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Milestones", "Only active contracts can reject milestones.");

    var milestone = _contractMilestones.FirstOrDefault(m => m.Id == milestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    return milestone.Reject(reason);
  }

  public Result<Updated> CompleteFromMilestones()
  {
    if (Type != ContractType.MilestoneBased)
      return Error.Conflict("Contracts.Status", "Only milestone-based contracts can be completed from milestones.");

    if (Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Status", "Only active contracts can be completed from milestones.");

    if (_contractMilestones.Any(m => m.Status != ContractMilestoneStatus.Approved))
      return Error.Conflict("Contracts.Status", "All milestones must be approved before completing the contract.");

    CompletionApprovedAt = DateTime.UtcNow;
    Status = ContractStatus.Completed;
    return Result.Updated;
  }

  public Result<Updated> ChangeStatus(ContractStatus newStatus)
  {
    if (Status == ContractStatus.Canceled)
      return Error.Failure("Contracts.Status", "Cannot change status of a cancelled contract.");

    if (Status == ContractStatus.Completed && newStatus != ContractStatus.Active)
      return Error.Failure("Contracts.Status", "Can only change status of a completed contract back to active.");

    Status = newStatus;
    return Result.Updated;
  }

  public Result<Updated> ChangeDeadline(DateTime newDeadline)
  {
    if (Type != ContractType.OneTime)
      return Error.Failure("Only one-time contracts can have their deadlines edited.");

    if (Status != ContractStatus.Active)
      return Error.Failure("Only active contracts can have their deadlines edited.");

    if (EndAt is null)
      return Error.Failure("Current deadline is not set. Cannot change deadline.");

    if (newDeadline <= EndAt)
      return Error.Validation("New deadline must be later than the current deadline.");

    EndAt = newDeadline;
    return Result.Updated;
  }


  public Result<Updated> RequestCompletion()
  {
    if (Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Status", "Only active contracts can be requested for completion.");

    CompletionRequestedAt = DateTime.UtcNow;
    Status = ContractStatus.PendingCompletion;
    return Result.Updated;
  }


  public Result<Updated> ApproveCompletion()
  {
    if (Status != ContractStatus.PendingCompletion)
      return Error.Conflict("Contracts.Status", "Only contracts pending completion can be approved for completion.");

    CompletionApprovedAt = DateTime.UtcNow;
    Status = ContractStatus.Completed;
    return Result.Updated;
  }

  public Result<Updated> RejectCompletion(string reason)
  {
    if (Status != ContractStatus.PendingCompletion)
      return Error.Conflict("Contracts.Status", "Only contracts pending completion can be rejected.");

    RejectionReason = reason;
    Status = ContractStatus.Active;
    return Result.Updated;
  }
}