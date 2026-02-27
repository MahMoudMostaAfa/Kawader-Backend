using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Jobs;

public class JobErrors
{
  public static Error MaxAttachmentsExceeded => Error.Validation("Maximum number of attachments (5) exceeded.");

  public static Error JobFileNotFound => Error.NotFound("Job file not found.");
  public static Error JobQuestionNotFound => Error.NotFound("Job question not found.");
  public static Error MaxQuestionsExceeded => Error.Validation("Maximum number of questions (5) exceeded.");

  public static Error JobSkillAlreadyAdded => Error.Validation("Skill already added to the job.");

  public static Error JobSkillNotFound => Error.NotFound("Skill not found in the job.");
}