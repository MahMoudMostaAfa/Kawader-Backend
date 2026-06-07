using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Jobs.SavedJobs;


public static class SavedJobErrors
{
  public static Error JobIdCannotBeEmpty => Error.Validation("JobId cannot be empty.");
  public static Error SavedByIdCannotBeEmpty => Error.Validation("SavedById cannot be empty.");
}