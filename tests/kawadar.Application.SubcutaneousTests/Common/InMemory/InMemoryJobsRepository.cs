using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.Jobs.JobReports.Enums;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryJobsRepository : IJobsRepository
{
    public readonly List<Job> Jobs = [];
    public readonly List<JobReport> JobReports = [];

    public Task AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        Jobs.Add(job);
        return Task.CompletedTask;
    }

    public void Delete(Job job)
    {
        Jobs.Remove(job);
    }

    public Task<Result<Job>> GetJobBySlugAsync(string slug)
    {
        var job = Jobs.FirstOrDefault(j => j.JobSlug == slug);
        return Task.FromResult(job is not null
            ? (Result<Job>)job
            : Error.NotFound("Job.NotFound", $"Job '{slug}' not found."));
    }

    public Task<Result<Job>> GetJobsAsync(Guid jobId)
    {
        var job = Jobs.FirstOrDefault(j => j.Id == jobId);
        return Task.FromResult(job is not null
            ? (Result<Job>)job
            : Error.NotFound("Job.NotFound", $"Job '{jobId}' not found."));
    }

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
        string sortBy)
    {
        var query = Jobs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(j => j.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                || j.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

        if (specilizationId.HasValue) query = query.Where(j => j.SpecilizationId == specilizationId.Value);
        if (jobType.HasValue) query = query.Where(j => j.JobType == jobType.Value);
        if (experienceLevel.HasValue) query = query.Where(j => j.ExperienceLevel == experienceLevel.Value);
        if (budgetRange.HasValue) query = query.Where(j => j.BudgetRange == budgetRange.Value);
        if (hourlyRateRange.HasValue) query = query.Where(j => j.HourlyRateRange == hourlyRateRange.Value);
        if (skillIds is { Count: > 0 })
            query = query.Where(j => j.Skills.Any(s => skillIds.Contains(s.Id)));

        // Apply sort before pagination
        query = sortBy?.ToLowerInvariant() switch
        {
            "oldest" => query.OrderBy(j => j.CreatedAt),
            _        => query.OrderByDescending(j => j.CreatedAt) // default: newest
        };

        var all = query.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<Job>(items, all.Count, page, pageSize));
    }

    public Task AddJobReport(JobReport jobReport, CancellationToken cancellationToken = default)
    {
        JobReports.Add(jobReport);
        return Task.CompletedTask;
    }

    public Task<Result<List<Job>>> GetJobsByIds(IEnumerable<Guid> Ids)
    {
        var idSet = Ids.ToHashSet();
        var found = Jobs.Where(j => idSet.Contains(j.Id)).ToList();
        return Task.FromResult<Result<List<Job>>>(found);
    }

    public Task<PaginatedList<JobReport>> GetJobReports(ReportType? reportType, ReportStatus? reportStatus, string sortBy, int page, int pageSize)
    {
        var query = JobReports.AsEnumerable();
        if (reportType.HasValue) query = query.Where(r => r.ReportType == reportType.Value);
        if (reportStatus.HasValue) query = query.Where(r => r.ReportStatus == reportStatus.Value);

        // Apply sort before pagination
        query = sortBy?.ToLowerInvariant() switch
        {
            "oldest" => query.OrderBy(r => r.CreatedAt),
            _        => query.OrderByDescending(r => r.CreatedAt)
        };

        var all = query.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<JobReport>(items, all.Count, page, pageSize));
    }

    public Task<Result<JobReport>> GetJobReportById(Guid Id)
    {
        var report = JobReports.FirstOrDefault(r => r.Id == Id);
        return Task.FromResult(report is not null
            ? (Result<JobReport>)report
            : Error.NotFound("JobReport.NotFound", $"JobReport '{Id}' not found."));
    }

    public Task<Result<Job>> GetJobByIdAsync(Guid Id)
    {
        var job = Jobs.FirstOrDefault(j => j.Id == Id);
        return Task.FromResult(job is not null
            ? (Result<Job>)job
            : Error.NotFound("Job.NotFound", $"Job '{Id}' not found."));
    }

    public Task<Result<List<JobReport>>> GetReportsByJobSlug(string slug)
    {
        var job = Jobs.FirstOrDefault(j => j.JobSlug == slug);
        if (job is null)
            return Task.FromResult<Result<List<JobReport>>>(Error.NotFound("Job.NotFound", $"Job with slug '{slug}' not found."));

        var reports = JobReports.Where(r => r.JobId == job.Id).ToList();
        return Task.FromResult<Result<List<JobReport>>>(reports);
    }

    public Task<Result<Dictionary<JobStatus, int>>> GetJobStatusDistribution()
    {
        var distribution = Jobs
            .GroupBy(j => j.JobStatus)
            .ToDictionary(g => g.Key, g => g.Count());
        return Task.FromResult<Result<Dictionary<JobStatus, int>>>(distribution);
    }

    public Task<Result<Dictionary<string, int>>> GetJobSpecilizationDistribution()
    {
        var distribution = Jobs
            .GroupBy(j => j.SpecilizationId.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
        return Task.FromResult<Result<Dictionary<string, int>>>(distribution);
    }

    public Task<Result<Dictionary<int, int>>> GetAverageJobPostingPerMonthDistribution()
    {
        // Group by (Year, Month) to get the count per calendar month across all years,
        // then average those counts per month-of-year (1–12).
        var averagePerMonth = Jobs
            .GroupBy(j => new { j.CreatedAt.Year, j.CreatedAt.Month })
            .GroupBy(g => g.Key.Month)
            .ToDictionary(
                mg => mg.Key,
                mg => (int)Math.Round(mg.Average(g => g.Count())));

        return Task.FromResult<Result<Dictionary<int, int>>>(averagePerMonth);
    }

    public void Clear()
    {
        Jobs.Clear();
        JobReports.Clear();
    }

    public Task<Result<PaginatedList<JobReport>>> GetReportsByJobSlug(string slug, ReportType? reportType, ReportStatus? reportStatus, int page, int pageSize, string sortBy)
    {
        throw new NotImplementedException();
    }
}
