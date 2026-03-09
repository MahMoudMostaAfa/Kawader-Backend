using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
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

  public async Task<Result<Job>> GetJobsAsync(Guid jobId)
  {
    var job = await _context.Jobs.Include(j => j.Attachments).Include(j => j.Specilization)
    .Include(J => J.Skills).Include(J => J.Questions.OrderBy(q => q.DisplayOrder))
     .FirstOrDefaultAsync(j => j.Id == jobId);

    if (job == null)
    {
      return Error.NotFound("Job not found.");
    }

    return job;
  }

  public void Delete(Job job)
  {
    _context.Jobs.Remove(job);
  }

  public async Task<Result<Job>> GetJobBySlugAsync(string slug)
  {
    var job = await _context.Jobs.Include(j => j.Attachments).Include(j => j.Specilization)
    .Include(J => J.Skills).Include(J => J.Questions.OrderBy(q => q.DisplayOrder))
     .FirstOrDefaultAsync(j => j.JobSlug == slug);

    if (job == null)
    {
      return Error.NotFound("Job not found.");
    }

    return job;
  }

  public async Task<PaginatedList<Job>> GetJobsAsync(
    string? search,
    Guid? specilizationId,
    JobType? jobType,
    JobExperienceLevel? experienceLevel,
    BudgetRange? budgetRange,
    HourlyRateRange? hourlyRateRange,
    List<Guid>? skillIds,
    int page,
    int pageSize,
    string sortBy)
  {
    var query = _context.Jobs
      .Include(j => j.Specilization)
      .Include(j => j.Skills)
      .AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
      query = query.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
    }

    if (specilizationId.HasValue)
    {
      query = query.Where(j => j.SpecilizationId == specilizationId.Value);
    }

    if (jobType.HasValue)
    {
      query = query.Where(j => j.JobType == jobType.Value);
    }

    if (experienceLevel.HasValue)
    {
      query = query.Where(j => j.ExperienceLevel == experienceLevel.Value);
    }

    if (budgetRange.HasValue)
    {
      query = query.Where(j => j.BudgetRange == budgetRange.Value);
    }

    if (hourlyRateRange.HasValue)
    {
      query = query.Where(j => j.HourlyRateRange == hourlyRateRange.Value);
    }

    if (skillIds is { Count: > 0 })
    {
      query = query.Where(j => j.Skills.Any(s => skillIds.Contains(s.Id)));
    }

    query = sortBy == "oldest"
      ? query.OrderBy(j => j.CreatedAt)
      : query.OrderByDescending(j => j.CreatedAt);

    var totalCount = await query.CountAsync();

    var items = await query
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();

    return new PaginatedList<Job>(items, totalCount, page, pageSize);
  }
}