using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Jobs.JobViews;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryJobViewRepository : IJobViewRepository
{
    public readonly List<JobView> JobViews = [];

    public Task AddAsync(JobView jobView)
    {
        JobViews.Add(jobView);
        return Task.CompletedTask;
    }

    public Task<bool> HasViewedAsync(Guid jobId, Guid userProfileId)
    {
        var viewed = JobViews.Any(v => v.JobId == jobId && v.UserProfileId == userProfileId);
        return Task.FromResult(viewed);
    }

    public Task<int> GetViewCountAsync(Guid jobId)
    {
        var count = JobViews.Count(v => v.JobId == jobId);
        return Task.FromResult(count);
    }

    public void Clear()
    {
        JobViews.Clear();
    }
}
