using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using Kawadar.Domain.Proposals.ProposalMilestones;
using Kawadar.Domain.Proposals.QuestionAnswers;

namespace Kawadar.Domain.Proposals;


public class JobProposal : AuditableEntity
{

  public Guid JobId { get; private set; }
  public Guid FreelancerId { get; private set; }

  public string CoverLetter { get; private set; }
  public JobProposalType ProposalType { get; private set; }

  // CASE OF ONE TIME
  public decimal? Amount { get; private set; }
  public int? EstimatedDays { get; private set; }

  // CASE OF MILESTONE BASED
  private readonly List<ProposalMilestone> _milestones = new();

  public IEnumerable<ProposalMilestone> Milestones => _milestones.AsReadOnly();

  // CASE OF HOURLY 

  public int? HourlyRate
  { get; private set; }
  public int? EstimatedHours { get; private set; }

  public JobProposalStatus Status { get; private set; }


  // proposal question answers

  private readonly List<ProposalQuestionAnswer> _questionAnswers = new();

  public IEnumerable<ProposalQuestionAnswer> QuestionAnswers => _questionAnswers.AsReadOnly();


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private JobProposal() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

  private JobProposal(Guid jobId, Guid freelancerId, string coverLetter, JobProposalType proposalType, decimal? amount, int? hourlyRate, int? estimatedHours, int? estimatedDays = null)
    : base(Guid.NewGuid())
  {
    JobId = jobId;
    FreelancerId = freelancerId;
    CoverLetter = coverLetter;
    ProposalType = proposalType;
    Amount = amount;
    HourlyRate = hourlyRate;
    EstimatedHours = estimatedHours;
    EstimatedDays = estimatedDays;
    Status = JobProposalStatus.Pending;

  }

  public static Result<JobProposal> Create(Guid jobId, Guid freelancerId, string coverLetter, JobProposalType proposalType, decimal? amount, int? hourlyRate, int? estimatedHours, int? estimatedDays = null)
  {
    if (string.IsNullOrWhiteSpace(coverLetter))
      return JobProposalErrors.CoverLetterRequired;

    if (Guid.Empty.Equals(jobId))
      return JobProposalErrors.JobIDRequired;

    if (Guid.Empty.Equals(freelancerId))
      return JobProposalErrors.FreelancerIDRequired;


    if (proposalType == JobProposalType.OneTime && (!amount.HasValue || amount.Value <= 0))
      return JobProposalErrors.AmountRequiredForOneTime;

    if (proposalType == JobProposalType.Hourly && (!estimatedHours.HasValue || estimatedHours.Value <= 0))
      return JobProposalErrors.EstimatedHoursRequiredForHourly;

    if (proposalType == JobProposalType.Hourly && (!hourlyRate.HasValue || hourlyRate.Value <= 0))
      return JobProposalErrors.AmountRequiredForOneTime;

    var proposal = new JobProposal(jobId, freelancerId, coverLetter, proposalType, amount, hourlyRate, estimatedHours, estimatedDays);
    return proposal;
  }


  public Result<Updated> Update(string? coverLetter, JobProposalType? proposalType, decimal? amount, int? hourlyRate, int? estimatedHours, int? estimatedDays = null)
  {
    if (!string.IsNullOrWhiteSpace(coverLetter))
      CoverLetter = coverLetter;

    if (proposalType.HasValue)
      ProposalType = proposalType.Value;

    if (amount.HasValue && amount.Value > 0)
      Amount = amount.Value;

    if (hourlyRate.HasValue && hourlyRate.Value > 0)
      HourlyRate = hourlyRate.Value;

    if (estimatedHours.HasValue && estimatedHours.Value > 0)
      EstimatedHours = estimatedHours.Value;

    if (estimatedDays.HasValue && estimatedDays.Value > 0)
      EstimatedDays = estimatedDays.Value;

    return Result.Updated;
  }


  public Result<Updated> AddMilestone(ProposalMilestone milestone)
  {
    if (ProposalType != JobProposalType.MilestoneBased)
      return Error.Validation("Milestones can only be added to milestone-based proposals.");
    _milestones.Add(milestone);
    return Result.Updated;
  }


  public Result<Deleted> RemoveMilestone(ProposalMilestone milestone)
  {
    if (ProposalType != JobProposalType.MilestoneBased)
      return Error.Validation("Milestones can only be removed from milestone-based proposals.");
    if (!_milestones.Contains(milestone))
      return Error.NotFound("Milestone not found.");

    _milestones.Remove(milestone);
    ReOrderMilestones();
    return Result.Deleted;
  }

  private void ReOrderMilestones()
  {
    for (int i = 0; i < _milestones.Count; i++)
    {
      _milestones[i].Update(null, null, null, null, null, i + 1);
    }
  }



  public Result<Updated> AddQuestionAnswer(ProposalQuestionAnswer questionAnswer)
  {
    _questionAnswers.Add(questionAnswer);
    return Result.Updated;
  }

  public Result<Deleted> RemoveQuestionAnswer(ProposalQuestionAnswer questionAnswer)
  {
    if (!_questionAnswers.Contains(questionAnswer))
      return Error.NotFound("Question answer not found.");

    _questionAnswers.Remove(questionAnswer);
    return Result.Deleted;
  }

  public Result<Updated> UpdateState(JobProposalStatus jobProposalStatus)
  {
    if (jobProposalStatus != Status) Status = jobProposalStatus;

    return Result.Updated;
  }


}