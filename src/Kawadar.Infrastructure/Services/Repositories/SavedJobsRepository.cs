using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.SavedJobs;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class SavedJobsRepository : ISavedJobsRepository
{
  private readonly AppDbContext _context;


  public SavedJobsRepository(AppDbContext context)
  {
    _context = context;
  }
  public async Task<Result<Created>> AddSavedJobAsync(SavedJob savedJob, CancellationToken cancellationToken = default)
  {

    var existingSavedJob = await _context.SavedJobs.FirstOrDefaultAsync(sj => sj.SavedById == savedJob.SavedById && sj.JobId == savedJob.JobId, cancellationToken);
    if (existingSavedJob != null)
    {
      return Error.Conflict("Job is already saved by the user.");
    }

    await _context.SavedJobs.AddAsync(savedJob);
    return Result.Created;

  }

  public async Task<Result<SavedJob>> GetSavedJobByUserIdAndJobIdAsync(Guid userId, Guid jobId, CancellationToken cancellationToken = default)
  {
    var savedJob = await _context.SavedJobs.FirstOrDefaultAsync(sj => sj.SavedById == userId && sj.JobId == jobId, cancellationToken);
    if (savedJob is null) return Error.NotFound("SavedJob.NotFound", "The saved job was not found for the given user and job IDs.");

    return savedJob;
  }

  public async Task<Result<PaginatedList<SavedJob>>> GetSavedJobsbyUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
  {
    var savedJobs = await _context.SavedJobs.Where(sj => sj.SavedById == userId)
    .Include(sj => sj.Job)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize).ToListAsync(cancellationToken);

    var totalSavedJobs = await _context.SavedJobs.CountAsync(sj => sj.SavedById == userId, cancellationToken);

    return new PaginatedList<SavedJob>(savedJobs, totalSavedJobs, pageNumber, pageSize);
  }

  public async Task<Result<bool>> IsJobSavedByUserAsync(Guid jobId, Guid userId, CancellationToken cancellationToken = default)
  {
    return await _context.SavedJobs.AnyAsync(sj => sj.JobId == jobId && sj.SavedById == userId, cancellationToken);

  }

  public async Task<Result<Deleted>> RemoveSavedJobAsync(SavedJob savedJob, CancellationToken cancellationToken = default)
  {
    _context.SavedJobs.Remove(savedJob);
    return Result.Deleted;
  }



}