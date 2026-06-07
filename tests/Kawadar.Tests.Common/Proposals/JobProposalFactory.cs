using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.Enums;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Tests.Common.Proposals;

public static class JobProposalFactory
{
  public static JobProposalBuilder Builder() => new();

  public static JobProposal CreateValidOneTime() =>
    Builder().WithProposalType(JobProposalType.OneTime).Build();

  public static JobProposal CreateValidMilestoneBased() =>
    Builder()
      .WithProposalType(JobProposalType.MilestoneBased)
      .WithoutAmount()
      .WithoutHourlyRate()
      .WithoutEstimatedHours()
      .WithEstimatedDays(null)
      .Build();

  public static JobProposal CreateValidHourly() =>
    Builder()
      .WithProposalType(JobProposalType.Hourly)
      .WithoutAmount()
      .WithHourlyRate(35)
      .WithEstimatedHours(12)
      .WithEstimatedDays(null)
      .Build();
}

public sealed class JobProposalBuilder
{
  private Guid _jobId = Guid.NewGuid();
  private Guid _freelancerId = Guid.NewGuid();
  private string _coverLetter = "Valid cover letter";
  private JobProposalType _proposalType = JobProposalType.OneTime;
  private decimal? _amount = 600m;
  private int? _hourlyRate;
  private int? _estimatedHours;
  private int? _estimatedDays = 5;

  public JobProposalBuilder WithJobId(Guid value)
  {
    _jobId = value;
    return this;
  }

  public JobProposalBuilder WithoutJobId() => WithJobId(Guid.Empty);

  public JobProposalBuilder WithFreelancerId(Guid value)
  {
    _freelancerId = value;
    return this;
  }

  public JobProposalBuilder WithoutFreelancerId() => WithFreelancerId(Guid.Empty);

  public JobProposalBuilder WithCoverLetter(string value)
  {
    _coverLetter = value;
    return this;
  }

  public JobProposalBuilder WithoutCoverLetter() => WithCoverLetter(string.Empty);

  public JobProposalBuilder WithProposalType(JobProposalType value)
  {
    _proposalType = value;
    return this;
  }

  public JobProposalBuilder WithAmount(decimal? value)
  {
    _amount = value;
    return this;
  }

  public JobProposalBuilder WithoutAmount() => WithAmount(null);

  public JobProposalBuilder WithHourlyRate(int? value)
  {
    _hourlyRate = value;
    return this;
  }

  public JobProposalBuilder WithoutHourlyRate() => WithHourlyRate(null);

  public JobProposalBuilder WithEstimatedHours(int? value)
  {
    _estimatedHours = value;
    return this;
  }

  public JobProposalBuilder WithoutEstimatedHours() => WithEstimatedHours(null);

  public JobProposalBuilder WithEstimatedDays(int? value)
  {
    _estimatedDays = value;
    return this;
  }

  public Result<JobProposal> BuildResult() =>
    JobProposal.Create(_jobId, _freelancerId, _coverLetter, _proposalType, _amount, _hourlyRate, _estimatedHours, _estimatedDays);

  public JobProposal Build()
  {
    var result = BuildResult();
    if (result.IsError)
    {
      throw new InvalidOperationException($"Could not build JobProposal: {result.TopError.Code} - {result.TopError.Description}");
    }

    return result.Value;
  }
}
