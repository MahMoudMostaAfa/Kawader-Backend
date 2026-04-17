using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.SavedJobs;

namespace Kawadar.Application.Common.Interfaces.Repositories;



public interface ISavedJobsRepository
{
  Task<Result<Created>> AddSavedJobAsync(SavedJob savedJob, CancellationToken cancellationToken = default);
  Task<Result<Deleted>> RemoveSavedJobAsync(SavedJob savedJob, CancellationToken cancellationToken = default);
  Task<Result<PaginatedList<SavedJob>>> GetSavedJobsbyUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

  Task<Result<bool>> IsJobSavedByUserAsync(Guid jobId, Guid userId, CancellationToken cancellationToken = default);

  Task<Result<SavedJob>> GetSavedJobByUserIdAndJobIdAsync(Guid userId, Guid jobId, CancellationToken cancellationToken = default);

}