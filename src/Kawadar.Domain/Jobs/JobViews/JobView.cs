using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Domain.Jobs.JobViews;

public class JobView : AuditableEntity
{
  public Guid JobId { get; private set; }
  public Job Job { get; private set; } = null!;

  public Guid UserProfileId { get; private set; }
  public UserProfile UserProfile { get; private set; } = null!;

#pragma warning disable CS8618
  private JobView() { }
#pragma warning restore CS8618

  private JobView(Guid jobId, Guid userProfileId) : base(Guid.NewGuid())
  {
    JobId = jobId;
    UserProfileId = userProfileId;
  }

  public static Result<JobView> Create(Guid jobId, Guid userProfileId)
  {
    if (jobId == Guid.Empty)
      return JobViewErrors.JobIdIsRequired;

    if (userProfileId == Guid.Empty)
      return JobViewErrors.UserProfileIdIsRequired;

    return new JobView(jobId, userProfileId);
  }
}
