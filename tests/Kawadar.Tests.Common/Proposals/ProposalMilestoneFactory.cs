using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.ProposalMilestones;

namespace Kawadar.Tests.Common.Proposals;

public static class ProposalMilestoneFactory
{
  public static ProposalMilestoneBuilder Builder() => new();

  public static ProposalMilestone CreateValid() => Builder().Build();
}

public sealed class ProposalMilestoneBuilder
{
  private Guid _jobProposalId = Guid.NewGuid();
  private string _title = "Initial milestone";
  private string _description = "Milestone description";
  private decimal _amount = 250m;
  private DateTime _dueDate = DateTime.UtcNow.AddDays(7);
  private int _displayOrder = 1;

  public ProposalMilestoneBuilder WithJobProposalId(Guid value)
  {
    _jobProposalId = value;
    return this;
  }

  public ProposalMilestoneBuilder WithTitle(string value)
  {
    _title = value;
    return this;
  }

  public ProposalMilestoneBuilder WithDescription(string value)
  {
    _description = value;
    return this;
  }

  public ProposalMilestoneBuilder WithAmount(decimal value)
  {
    _amount = value;
    return this;
  }

  public ProposalMilestoneBuilder WithDueDate(DateTime value)
  {
    _dueDate = value;
    return this;
  }

  public ProposalMilestoneBuilder WithDisplayOrder(int value)
  {
    _displayOrder = value;
    return this;
  }

  public Result<ProposalMilestone> BuildResult() =>
    ProposalMilestone.Create(_jobProposalId, _title, _description, _amount, _dueDate, _displayOrder);

  public ProposalMilestone Build()
  {
    var result = BuildResult();
    if (result.IsError)
    {
      throw new InvalidOperationException($"Could not build ProposalMilestone: {result.TopError.Code} - {result.TopError.Description}");
    }

    return result.Value;
  }
}
