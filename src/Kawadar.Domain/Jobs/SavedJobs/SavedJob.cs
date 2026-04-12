using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Jobs.SavedJobs;


public class SavedJob : AuditableEntity
{

  public Guid JobId { get; private set; }
  public Job Job { get; private set; } = null!;
  public Guid SavedById { get; private set; }


  private SavedJob() { }
  private SavedJob(Guid jobId, Guid savedById) : base(Guid.NewGuid())
  {
    JobId = jobId;
    SavedById = savedById;
  }


  public static Result<SavedJob> Create(Guid jobId, Guid savedById)
  {
    if (jobId == Guid.Empty)
    {
      return SavedJobErrors.JobIdCannotBeEmpty;
    }

    if (savedById == Guid.Empty)
    {
      return SavedJobErrors.SavedByIdCannotBeEmpty;
    }
    return new SavedJob(jobId, savedById);
  }


}