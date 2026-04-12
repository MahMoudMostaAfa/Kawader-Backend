using Kawadar.Application.Features.Proposals.Dtos;

namespace Kawadar.Api.Requests.Proposals;

public class UpdateProposalRequest
{
  public string? CoverLetter { set; get; }
  public List<QuestionAnswerUpdateDto>? QuestionAnswerUpdateDtos { set; get; }
  public List<MilestoneUpdateDto>? MilestoneUpdateDtos { set; get; }
  public decimal? Amount { set; get; }
  public int? EstimatedDays { set; get; }
  public int? HourlyRate { set; get; }
  public int? EstimatedHours { set; get; }
}