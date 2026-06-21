using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.Jobs.JobReports.Enums;
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
      .Where(j => !j.IsPrivate)
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

  public async Task AddJobReport(JobReport jobReport, CancellationToken cancellationToken = default)
  {
    await _context.JobReports.AddAsync(jobReport, cancellationToken);
  }

  public async Task<PaginatedList<JobReport>> GetJobReports(ReportType? reportType, ReportStatus? reportStatus, string sortBy, int page, int pageSize)
  {
    var query = _context.JobReports.AsQueryable();

    if (reportType.HasValue)
    {
      query = query.Where(x => x.ReportType == reportType);
    }

    if (reportStatus.HasValue)
    {
      query = query.Where(x => x.ReportStatus == reportStatus);
    }

    query = sortBy == "oldest" ? query.OrderBy(j => j.CreatedAt) : query.OrderByDescending(j => j.CreatedAt);

    var totalCount = await query.CountAsync();

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PaginatedList<JobReport>(items, totalCount, page, pageSize);
  }

  public async Task<Result<JobReport>> GetJobReportById(Guid Id)
  {
    var jobReport = await _context.JobReports.FirstOrDefaultAsync(x => x.Id == Id);
    if (jobReport is null) return Error.NotFound("Job Report not found");

    return jobReport;
  }

  public async Task<Result<Job>> GetJobByIdAsync(Guid Id)
  {
    var job = await _context.Jobs.Include(j => j.Attachments).Include(j => j.Specilization)
    .Include(J => J.Skills).Include(J => J.Questions.OrderBy(q => q.DisplayOrder))
     .FirstOrDefaultAsync(j => j.Id == Id);

    if (job == null)
    {
      return Error.NotFound("Job not found.");
    }

    return job;
  }

  public async Task<Result<List<Job>>> GetJobsByIds(IEnumerable<Guid> Ids)
  {
    List<Job> jobs = new();
    foreach (var id in Ids)
    {
      var job = await _context.Jobs.FirstOrDefaultAsync(x => x.Id == id);
      if (job is null) continue;

      jobs.Add(job);
    }
    return jobs;
  }

  public async Task<Result<PaginatedList<JobReport>>> GetReportsByJobSlug(string slug, ReportType? reportType, ReportStatus? reportStatus, int page, int pageSize, string sortBy)
  {
    var query = _context.JobReports.AsQueryable();

    var job = await _context.Jobs.Where(x => x.JobSlug == slug).FirstOrDefaultAsync();

    if (job is null)
    {
      return Error.NotFound("No such job with this slug exists");
    }

    query = query.Where(x => x.JobId == job.Id);
    if (reportType.HasValue)
    {
      query = query.Where(x => x.ReportType == reportType);
    }

    if (reportStatus.HasValue)
    {
      query = query.Where(x => x.ReportStatus == reportStatus);
    }

    query = sortBy == "oldest" ? query.OrderBy(j => j.CreatedAt) : query.OrderByDescending(j => j.CreatedAt);

    var totalCount = await query.CountAsync();

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PaginatedList<JobReport>(items, totalCount, page, pageSize);
  }

  //If there is no job with one of the status there won't be an entry for it in the dictionary
  public async Task<Result<Dictionary<JobStatus, int>>> GetJobStatusDistribution()
  {
    var JobStatusDistribution = await _context.Jobs.GroupBy(x => x.JobStatus).ToDictionaryAsync(x => x.Key, x => x.Count());
    return JobStatusDistribution;
  }

  public async Task<Result<Dictionary<string, int>>> GetJobSpecilizationDistribution()
  {
    var jobSpecilizationDistribution = await _context.Jobs.GroupBy(x => x.Specilization.Name).ToDictionaryAsync(x => x.Key, x => x.Count());
    return jobSpecilizationDistribution;
  }

  public async Task<Result<Dictionary<int, int>>> GetAverageJobPostingPerMonthDistribution()
  {
    var JobPostings = await _context.Jobs.Where(x => x.CreatedAt.Year == DateTime.UtcNow.Year)
        .GroupBy(x => x.CreatedAt.Month).ToDictionaryAsync(x => x.Key, x => x.Count());
    return JobPostings;
  }
}