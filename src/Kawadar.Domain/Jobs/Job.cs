using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Specilizations;


namespace Kawadar.Domain.Jobs;

public class Job : AuditableEntity
{

  public Guid PostedById { get; private set; }

  public Guid SpecilizationId { get; private set; }

  public Specilization Specilization { get; private set; } = null!;

  public string Title { get; private set; } = "";

  public string Description { get; private set; } = "";
  public JobType JobType { get; private set; }

  public JobStatus JobStatus { get; private set; } = JobStatus.Open;

  public BudgetRange BudgetRange { get; private set; }

  public JobExperienceLevel ExperienceLevel { get; private set; } = JobExperienceLevel.EntryLevel;

  public HourlyRateRange HourlyRateRange { get; private set; } = HourlyRateRange.LessThan100;

  public int DurationInDays { get; private set; }

  public string JobSlug { get; private set; } = "";


  // Job Attachments

  private readonly List<JobFile> _attachments = [];
  public IReadOnlyList<JobFile> Attachments => _attachments.AsReadOnly();

  // Job Questions
  private readonly List<JobQuestion> _questions = [];
  public IReadOnlyList<JobQuestion> Questions => _questions.AsReadOnly();

  // Job Skills
  private readonly List<Skill> _skills = [];
  public IReadOnlyList<Skill> Skills => _skills.AsReadOnly();


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private Job() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private Job(Guid postedById, Guid specilizationId, string title, string description, JobType jobType, BudgetRange budgetRange, HourlyRateRange hourlyRateRange, int durationInDays, string jobSlug, JobExperienceLevel experienceLevel) : base(Guid.NewGuid())
  {
    PostedById = postedById;
    SpecilizationId = specilizationId;
    Title = title;
    Description = description;
    JobType = jobType;
    BudgetRange = budgetRange;
    HourlyRateRange = hourlyRateRange;
    DurationInDays = durationInDays;
    JobSlug = jobSlug;
    ExperienceLevel = experienceLevel;
  }


  public static Result<Job> Create(Guid postedById, Guid specilizationId, string title, string description, JobType jobType, BudgetRange budgetRange, HourlyRateRange hourlyRateRange, int durationInDays, string jobSlug)
  {

    return new Job(postedById, specilizationId, title, description, jobType, budgetRange, hourlyRateRange, durationInDays, jobSlug, JobExperienceLevel.EntryLevel);
  }

  public Result<Updated> Update(string title, string description, JobType jobType, BudgetRange budgetRange, HourlyRateRange hourlyRateRange, int durationInDays, JobExperienceLevel experienceLevel)
  {
    Title = title;
    Description = description;
    JobType = jobType;
    BudgetRange = budgetRange;
    HourlyRateRange = hourlyRateRange;
    DurationInDays = durationInDays;
    ExperienceLevel = experienceLevel;

    return Result.Updated;
  }

  // Job Attachments Management
  public Result<Updated> AddAttachment(JobFile jobFile)
  {
    if (_attachments.Count >= 5)
      return JobErrors.MaxAttachmentsExceeded;
    _attachments.Add(jobFile);

    return Result.Updated;
  }

  public Result<Deleted> RemoveAttachment(Guid jobFileId)
  {
    var attachment = _attachments.FirstOrDefault(a => a.Id == jobFileId);
    if (attachment == null)
      return JobErrors.JobFileNotFound;

    _attachments.Remove(attachment);

    return Result.Deleted;
  }

  // Job Questions Management
  public Result<Updated> AddQuestion(JobQuestion jobQuestion)
  {

    if (_questions.Count >= 5)
    {
      return JobErrors.MaxQuestionsExceeded;
    }

    _questions.Add(jobQuestion);
    return Result.Updated;
  }

  public Result<Deleted> RemoveQuestion(Guid jobQuestionId)
  {
    var question = _questions.FirstOrDefault(q => q.Id == jobQuestionId);
    if (question == null)
    {
      return JobErrors.JobQuestionNotFound;
    }

    _questions.Remove(question);

    return Result.Deleted;
  }

  // Job Skills Management
  public Result<Updated> AddSkill(Skill skill)
  {
    if (_skills.Any(s => s.Id == skill.Id))
    {
      return JobErrors.JobSkillAlreadyAdded;
    }

    _skills.Add(skill);
    return Result.Updated;
  }

  public Result<Deleted> RemoveSkill(Guid skillId)
  {
    var skill = _skills.FirstOrDefault(s => s.Id == skillId);
    if (skill == null)
    {
      return JobErrors.JobSkillNotFound;
    }

    _skills.Remove(skill);
    return Result.Deleted;
  }

}