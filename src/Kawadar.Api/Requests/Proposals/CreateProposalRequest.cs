using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Proposals.Enums;

namespace Kawadar.Api.Requests.Proposals;

public class CreateProposalRequest
{
  public string CoverLetter { set; get; } = string.Empty;
  public JobProposalType JobProposalType { set; get; }
  public decimal? Amount { get; set; }

  public int? EstimatedDays { get; set; }

  public int? HourlyRate { set; get; }

  public int? EstimatedHours { set; get; }

  public List<QuestionAnswerDto>? QuestionAnswerDtos { set; get; }

  public List<MilestoneDto>? MilestoneDtos { get; set; }




}