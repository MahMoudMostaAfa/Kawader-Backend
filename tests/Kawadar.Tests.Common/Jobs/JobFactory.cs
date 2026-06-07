using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Skills;

namespace Kawadar.Tests.Common.Jobs;

public static class JobFactory
{
  public static JobBuilder Builder() => new();

  public static Job CreateValid() => Builder().Build();

  public static Skill CreateSkill(string name = "Skill")
  {
    var result = Skill.Create(name, true, Guid.NewGuid());
    if (result.IsError)
    {
      throw new InvalidOperationException($"Could not build Skill: {result.TopError.Code} - {result.TopError.Description}");
    }

    return result.Value;
  }

  public static JobQuestion CreateQuestion(string question = "What is your approach?", bool isRequired = false, int displayOrder = 1)
  {
    var result = JobQuestion.Create(question, isRequired, displayOrder);
    if (result.IsError)
    {
      throw new InvalidOperationException($"Could not build JobQuestion: {result.TopError.Code} - {result.TopError.Description}");
    }

    return result.Value;
  }

  public static Domain.Common.ValueObjects.FileInfo CreateFileInfo(
    string fileName = "brief.pdf",
    string fileUrl = "/uploads/jobs/brief.pdf",
    long fileSizeInBytes = 100,
    string mimeType = "application/pdf")
  {
    return new Domain.Common.ValueObjects.FileInfo
    {
      FileName = fileName,
      FileUrl = fileUrl,
      FileSizeInBytes = fileSizeInBytes,
      MimeType = mimeType,
    };
  }

  public static JobFile CreateAttachment(
    string fileName = "brief.pdf",
    string fileUrl = "/uploads/jobs/brief.pdf",
    long fileSizeInBytes = 100,
    string mimeType = "application/pdf")
  {
    var fileInfo = CreateFileInfo(fileName, fileUrl, fileSizeInBytes, mimeType);

    var result = JobFile.Create(fileInfo);
    if (result.IsError)
    {
      throw new InvalidOperationException($"Could not build JobFile: {result.TopError.Code} - {result.TopError.Description}");
    }

    return result.Value;
  }
}

public sealed class JobBuilder
{
  private Guid _postedById = Guid.NewGuid();
  private Guid _specilizationId = Guid.NewGuid();
  private string _title = "Senior Backend Developer";
  private string _description = "Need an experienced developer for APIs.";
  private JobType _jobType = JobType.FixedPrice;
  private BudgetRange _budgetRange = BudgetRange.From1000To5000;
  private HourlyRateRange _hourlyRateRange = HourlyRateRange.From100To200;
  private int _durationInDays = 14;
  private JobExperienceLevel _experienceLevel = JobExperienceLevel.MidLevel;
  private string _jobSlug = "senior-backend-developer-test";
  private List<JobQuestion> _questions = [];
  private List<Skill> _skills = [];
  private List<JobFile> _attachments = [];

  public JobBuilder WithPostedById(Guid value)
  {
    _postedById = value;
    return this;
  }

  public JobBuilder WithSpecilizationId(Guid value)
  {
    _specilizationId = value;
    return this;
  }

  public JobBuilder WithTitle(string value)
  {
    _title = value;
    return this;
  }

  public JobBuilder WithDescription(string value)
  {
    _description = value;
    return this;
  }

  public JobBuilder WithJobType(JobType value)
  {
    _jobType = value;
    return this;
  }

  public JobBuilder WithBudgetRange(BudgetRange value)
  {
    _budgetRange = value;
    return this;
  }

  public JobBuilder WithHourlyRateRange(HourlyRateRange value)
  {
    _hourlyRateRange = value;
    return this;
  }

  public JobBuilder WithDurationInDays(int value)
  {
    _durationInDays = value;
    return this;
  }

  public JobBuilder WithExperienceLevel(JobExperienceLevel value)
  {
    _experienceLevel = value;
    return this;
  }

  public JobBuilder WithSlug(string value)
  {
    _jobSlug = value;
    return this;
  }

  public JobBuilder WithQuestions(List<JobQuestion> value)
  {
    _questions = value;
    return this;
  }

  public JobBuilder WithoutQuestions() => WithQuestions([]);

  public JobBuilder WithSkills(List<Skill> value)
  {
    _skills = value;
    return this;
  }

  public JobBuilder WithoutSkills() => WithSkills([]);

  public JobBuilder WithAttachments(List<JobFile> value)
  {
    _attachments = value;
    return this;
  }

  public JobBuilder WithoutAttachments() => WithAttachments([]);

  public Result<Job> BuildResult() =>
    Job.Create(
      _postedById,
      _specilizationId,
      _title,
      _description,
      _jobType,
      _budgetRange,
      _hourlyRateRange,
      _durationInDays,
      _experienceLevel,
      _jobSlug,
      _questions,
      _skills,
      _attachments);

  public Job Build()
  {
    var result = BuildResult();
    if (result.IsError)
    {
      throw new InvalidOperationException($"Could not build Job: {result.TopError.Code} - {result.TopError.Description}");
    }

    return result.Value;
  }
}
