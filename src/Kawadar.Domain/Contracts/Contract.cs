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