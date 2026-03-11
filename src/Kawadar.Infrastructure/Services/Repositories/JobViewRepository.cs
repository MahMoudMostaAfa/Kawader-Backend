using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Jobs.JobViews;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class JobViewRepository : IJobViewRepository
{
  private readonly AppDbContext _context;

  public JobViewRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task AddAsync(JobView jobView)
  {
    await _context.JobViews.AddAsync(jobView);
  }

  public async Task<bool> HasViewedAsync(Guid jobId, Guid userProfileId)
  {
    return await _context.JobViews
      .AnyAsync(v => v.JobId == jobId && v.UserProfileId == userProfileId);
  }

  public async Task<int> GetViewCountAsync(Guid jobId)
  {
    return await _context.JobViews
      .CountAsync(v => v.JobId == jobId);
  }
}
