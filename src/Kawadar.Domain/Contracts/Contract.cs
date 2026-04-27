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




  public DateTime StartAt { get; private set; }
  public DateTime? EndAt { get; private set; }

  private readonly List<ContractMilestone> _contractMilestones = [];

  public IReadOnlyList<ContractMilestone> ContractMilestones => _contractMilestones.AsReadOnly();

  private Contract()
  {

  }

  private Contract(Guid jobId, Guid proposalId, Guid clientId, Guid freelancerId, ContractType type, DateTime startAt, DateTime? endAt, decimal? oneTimeFixedPrice = null)
  {
    JobId = jobId;
    ProposalId = proposalId;
    ClientId = clientId;
    FreelancerId = freelancerId;
    Type = type;
    StartAt = startAt;
    EndAt = endAt;
    OneTimeFixedPrice = oneTimeFixedPrice;

  }

  public static Result<Contract> Create(Guid jobId, Guid proposalId, Guid clientId, Guid freelancerId, ContractType type, DateTime startAt, DateTime? endAt, decimal? oneTimeFixedPrice = null)
  {
    return new Contract(jobId, proposalId, clientId, freelancerId, type, startAt, endAt, oneTimeFixedPrice);
  }

  public void AddContractMilestone(Guid ProposalMilestoneId, string title, string description, decimal amount, DateTime dueDate)
  {
    var order = _contractMilestones.Count + 1;
    var contractMilestoneResult = ContractMilestone.Create(Id, ProposalMilestoneId, title, description, amount, order, dueDate);

    var contractMilestone = contractMilestoneResult.Value;

    _contractMilestones.Add(contractMilestone);


  }

}