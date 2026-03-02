using Kawadar.Domain.Jobs.Enums;

namespace Kawadar.Api.Requests.Job;

public class UpdateJobRequest
{
  public string? Title { get; set; }
  public string? Description { get; set; }
  public Guid? SpecilizationId { get; set; }
  public JobType? JobType { get; set; }
  public BudgetRange? BudgetRange { get; set; }
  public HourlyRateRange? HourlyRateRange { get; set; }
  public int? DurationInDays { get; set; }
  public JobExperienceLevel? ExperienceLevel { get; set; }

}