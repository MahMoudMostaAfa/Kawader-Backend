namespace Kawadar.Domain.Jobs;

using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Specilizations;



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
  private Job(Guid postedById, Guid specilizationId, string title, string description, JobType jobType, BudgetRange budgetRange, HourlyRateRange hourlyRateRange, int durationInDays, string jobSlug, JobExperienceLevel experienceLevel, List<JobQuestion> questions, List<Skill> skills, List<JobFile> attachments) : base(Guid.NewGuid())
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
    _questions = questions;
    _skills = skills;
    _attachments = attachments;
  }


  public static Result<Job> Create(Guid postedById, Guid specilizationId, string title, string description, JobType jobType, BudgetRange budgetRange, HourlyRateRange hourlyRateRange, int durationInDays, JobExperienceLevel jobExperienceLevel, string jobSlug, List<JobQuestion> questions, List<Skill> skills, List<JobFile> attachments)
  {

    return new Job(postedById, specilizationId, title, description, jobType, budgetRange, hourlyRateRange, durationInDays, jobSlug, jobExperienceLevel, questions, skills, attachments);
  }

  public Result<Updated> Update(string? title, string? description, JobType? jobType, BudgetRange? budgetRange, HourlyRateRange? hourlyRateRange, int? durationInDays, JobExperienceLevel? experienceLevel, Guid? specilizationId)
  {
    if (!string.IsNullOrWhiteSpace(title) && title != Title)
    {
      var slugResult = GenerateSlug(title);
      if (slugResult.IsError) return slugResult.Errors;
      JobSlug = slugResult.Value;
      Title = title;
    }
    if (!string.IsNullOrWhiteSpace(description)) Description = description;
    if (jobType.HasValue) JobType = jobType.Value;
    if (budgetRange.HasValue) BudgetRange = budgetRange.Value;
    if (hourlyRateRange.HasValue) HourlyRateRange = hourlyRateRange.Value;
    if (durationInDays.HasValue) DurationInDays = durationInDays.Value;
    if (experienceLevel.HasValue) ExperienceLevel = experienceLevel.Value;

    if (specilizationId.HasValue && !Guid.Equals(SpecilizationId, specilizationId.Value)) SpecilizationId = specilizationId.Value;

    return Result.Updated;


  }

  // Job Attachments Management

  public static Result<string> GenerateSlug(string title)
  {
    // title is arabic or english or both, we need to generate a slug that is URL-friendly and unique. We can use a combination of the title and a unique identifier (like a timestamp or GUID) to ensure uniqueness.
    var slugBase = title.ToLower().Replace(" ", "-").Replace(".", "").Replace(",", "");
    var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
    var slug = $"{slugBase}-{uniqueId}";

    return slug;
  }
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
    if (_skills.Count >= 10)
    {
      return JobErrors.MaxSkillsExceeded;
    }
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