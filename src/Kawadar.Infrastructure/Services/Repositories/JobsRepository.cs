using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class JobsRepository : IJobsRepository
{
  private readonly AppDbContext _context;

  public JobsRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task AddAsync(Job job, CancellationToken cancellationToken = default)
  {
    await _context.Jobs.AddAsync(job, cancellationToken);
  }

  public async Task<Result<Job>> GetJobBySlugAsync(string slug)
  {
    var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobSlug == slug);

    if (job == null)
    {
      return Error.NotFound("Job not found.");
    }

    return job;
  }
}