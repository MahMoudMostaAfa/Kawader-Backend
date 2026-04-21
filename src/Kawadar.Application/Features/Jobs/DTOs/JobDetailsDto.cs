namespace Kawadar.Application.Features.Jobs.DTOs;

using Kawadar.Domain.Jobs.Enums;

public class JobDetailsDto
{
  public string Title { get; set; } = null!;
  public string Description { get; set; } = null!;
  public string PosterFullName { get; set; } = null!;
  public Guid JobId { get; set; } 
  public string PosterProfilePictureUrl { get; set; } = null!;
  public string PosterUsername { get; set; } = null!;
  public string JobSlug { get; set; } = null!;

  public List<JobQuestionDto> Questions { get; set; } = null!;
  public List<JobSkillDto> Skills { get; set; } = null!;
  public List<JobAttachmentDto> Attachments { get; set; } = null!;
  public string Specilization { get; set; } = null!;
  public JobType JobType { get; set; }
  public BudgetRange? BudgetRange { get; set; }
  public HourlyRateRange? HourlyRateRange { get; set; }
  public int DurationInDays { get; set; }
  public JobExperienceLevel ExperienceLevel { get; set; }
  public JobStatus JobStatus { get; set; }
}

public class JobQuestionDto
{
  public string QuestionText { get; set; } = null!;
  public Guid Id { get; set; }

  public bool IsRequired { get; set; }
  public int DisplayOrder { get; set; }
}

public class JobSkillDto
{
  public string SkillName { get; set; } = null!;
  public Guid Id { get; set; }
}


public class JobAttachmentDto
{
  public string FileName { get; set; } = null!;
  public string FileUrl { get; set; } = null!;
  public string ContentType { get; set; } = null!;
  public long FileSizeInBytes { get; set; }
  public Guid Id { get; set; }
}