using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Proposals.ProposalMilestones;


public class ProposalMilestone : AuditableEntity
{
  public Guid JobProposalId { get; private set; }

  public string Title { get; private set; } = string.Empty;

  public string Description { get; private set; } = string.Empty;

  public decimal Amount { get; private set; }
  public DateTime DueDate { get; private set; }

  public ProposalMilestoneStatus Status { get; private set; }

  public int DisplayOrder { get; private set; }

  private ProposalMilestone() { }


  private ProposalMilestone(Guid jobProposalId, string title, string description, decimal amount, DateTime dueDate, int displayOrder = 1) : base(Guid.NewGuid())
  {
    JobProposalId = jobProposalId;
    Title = title;
    Description = description;
    Amount = amount;
    DueDate = dueDate;
    Status = ProposalMilestoneStatus.Pending;
    DisplayOrder = displayOrder;
  }

  public static Result<ProposalMilestone> Create(Guid jobProposalId, string title, string description, decimal amount, DateTime dueDate, int displayOrder = 1)
  {
    if (string.IsNullOrWhiteSpace(title))
      return Error.Validation("Title is required.");

    if (amount <= 0)
      return Error.Validation("Amount must be greater than zero.");

    if (dueDate <= DateTime.UtcNow)
      return Error.Validation("Due date must be in the future.");

    var milestone = new ProposalMilestone(jobProposalId, title, description, amount, dueDate, displayOrder);
    return milestone;
  }


  public Result<Updated> Update(string? title, string? description, decimal? amount, DateTime? dueDate, ProposalMilestoneStatus? status, int? displayOrder)
  {
    if (!string.IsNullOrWhiteSpace(title))
      Title = title;

    if (!string.IsNullOrWhiteSpace(description))
      Description = description;

    if (amount.HasValue && amount.Value > 0)
      Amount = amount.Value;

    if (dueDate.HasValue && dueDate.Value > DateTime.UtcNow)
      DueDate = dueDate.Value;

    if (status.HasValue)
      Status = status.Value;

    if (displayOrder.HasValue)
      DisplayOrder = displayOrder.Value;

    return Result.Updated;
  }



}




