namespace Kawadar.Application.Features.Jobs.DTOs;

using Kawadar.Domain.Jobs.Enums;

public class JobSummaryDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = null!;
  public string Description { get; set; } = null!;
  public string JobSlug { get; set; } = null!;
  public string Specilization { get; set; } = null!;
  public JobType JobType { get; set; }
  public BudgetRange? BudgetRange { get; set; }
  public HourlyRateRange? HourlyRateRange { get; set; }
  public int DurationInDays { get; set; }
  public JobExperienceLevel ExperienceLevel { get; set; }
  public JobStatus JobStatus { get; set; }
  public List<JobSkillDto> Skills { get; set; } = [];
  public DateTime CreatedAt { get; set; }
}
