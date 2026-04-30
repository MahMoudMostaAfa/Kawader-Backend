using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IJobsRepository
{
  public Task AddAsync(Job job, CancellationToken cancellationToken = default);
  public void Delete(Job job);
  public Task<Result<Job>> GetJobBySlugAsync(string slug);

  public Task<Result<Job>> GetJobsAsync(Guid jobId);
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
  public Task AddJobReport(JobReport jobReport, CancellationToken cancellationToken = default);

  public Task<Result<List<Job>>> GetJobsByIds(IEnumerable<Guid> Ids);
  public Task<PaginatedList<JobReport>> GetJobReports(ReportType? reportType, ReportStatus? reportStatus, string sortBy, int page, int pageSize);
  public Task<Result<JobReport>> GetJobReportById(Guid Id);
  public Task<Result<Job>> GetJobByIdAsync(Guid Id);
  public Task<Result<List<JobReport>>> GetReportsByJobSlug(string slug);
  public Task<Result<Dictionary<JobStatus, int>>> GetJobStatusDistribution();
  public Task<Result<Dictionary<string, int>>> GetJobSpecilizationDistribution();
  public Task<Result<Dictionary<int, int>>> GetAverageJobPostingPerMonthDistribution();
}