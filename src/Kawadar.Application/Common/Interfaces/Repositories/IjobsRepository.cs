using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IJobsRepository
{
  public Task AddAsync(Job job, CancellationToken cancellationToken = default);
  public Task<Result<Job>> GetJobBySlugAsync(string slug);
}