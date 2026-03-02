using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Jobs.JobViews;

public static class JobViewErrors
{
  public static Error JobIdIsRequired => Error.Validation("Job ID is required.");
  public static Error UserProfileIdIsRequired => Error.Validation("User profile ID is required.");
}
