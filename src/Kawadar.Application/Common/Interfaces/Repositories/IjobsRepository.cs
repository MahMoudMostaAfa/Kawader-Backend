using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IJobsRepository
{
  public Task AddAsync(Job job, CancellationToken cancellationToken = default);
  public Task<Result<Job>> GetJobBySlugAsync(string slug);
  public Task<PaginatedList<Job>> GetJobsAsync(
    string? search,
    Guid? specilizationId,
    JobType? jobType,
    JobExperienceLevel? experienceLevel,
    BudgetRange? budgetRange,
    HourlyRateRange? hourlyRateRange,
    List<Guid>? skillIds,
    int page,
    int pageSize,
    string sortBy
  );
}