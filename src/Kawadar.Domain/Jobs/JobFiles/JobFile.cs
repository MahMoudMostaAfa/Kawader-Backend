using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Common.ValueObjects;

namespace Kawadar.Domain.Jobs.JobFiles;

public class JobFile : AuditableEntity
{


  public Common.ValueObjects.FileInfo File { get; private set; }



#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private JobFile() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  private JobFile(Common.ValueObjects.FileInfo file) : base(Guid.NewGuid())
  {
    File = file;

  }

  public static Result<JobFile> Create(Common.ValueObjects.FileInfo file)
  {
    return new JobFile(file);
  }
}