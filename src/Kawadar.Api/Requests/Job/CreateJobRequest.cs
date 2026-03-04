using Kawadar.Domain.Jobs.Enums;

namespace Kawadar.Api.Requests.Job;

public class CreateJobRequest
{
  public string Title { get; set; } = "";
  public string Description { get; set; } = "";
  public Guid SpecilizationId { get; set; }

  /// <summary>0 = FullTime, 1 = PartTime, 2 = Contract (send as integer)</summary>
  public int JobType { get; set; }

  /// <summary>Send as integer value of the BudgetRange enum</summary>
  public int BudgetRange { get; set; }

  /// <summary>Send as integer value of the HourlyRateRange enum</summary>
  public int HourlyRateRange { get; set; }

  public int DurationInDays { get; set; }

  /// <summary>Send as integer value of the JobExperienceLevel enum</summary>
  public int ExperienceLevel { get; set; }

  /// <summary>Question texts. Use same index as QuestionsRequired. e.g. Questions[0]=What is your experience?</summary>
  public List<string>? Questions { get; set; }

  /// <summary>Whether each question is required. Same index as Questions. e.g. QuestionsRequired[0]=true</summary>
  public List<bool>? QuestionsRequired { get; set; }
  /// <summary>Comma-separated list of Skill GUIDs e.g. "guid1,guid2"</summary>
  public string? SkillIds { get; set; }

  /// <summary>Uploaded file attachments (zip / image / pdf). Max 5 total combined with links.</summary>
  public List<IFormFile>? AttachmentFiles { get; set; }

  /// <summary>External URL attachments. Max 5 total combined with files.</summary>
  public List<string>? AttachmentLinks { get; set; }
}
