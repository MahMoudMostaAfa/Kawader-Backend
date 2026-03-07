using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobViews;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IJobViewRepository
{
  Task AddAsync(JobView jobView);
  Task<bool> HasViewedAsync(Guid jobId, Guid userProfileId);
  Task<int> GetViewCountAsync(Guid jobId);
}
