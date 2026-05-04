using Kawadar.Domain.Proposals.Enums;

namespace Kawadar.Application.Features.Proposals.Dtos;

public class ProposalDetailsDto
{

  public Guid JobId { set; get; }
  public string CoverLetter { set; get; } = string.Empty;

  public JobProposalType JobProposalType { set; get; }

  public string? ProposalByUserName { get; set; }
  public string? ProposalByFullName { get; set; }
  public string? ProposalByPhoto { set; get; }
  public decimal? Amount { get; set; }

  public int? EstimatedDays { get; set; }

  public int? HourlyRate { set; get; }

  public int? EstimatedHours { set; get; }

  public List<MilestoneDto>? Milestones { get; set; }


  public List<QuestionWithAnswerDto>? QuestionsWithAnswer { get; set; }
  public DateTime SubmittedAt { get; set; }


}

public class QuestionWithAnswerDto
{
  public string Question { set; get; } = string.Empty;

  public string Answer { get; set; } = string.Empty;
}