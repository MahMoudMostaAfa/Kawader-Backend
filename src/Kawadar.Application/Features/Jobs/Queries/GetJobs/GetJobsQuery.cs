using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobs;

public record GetJobsQuery(
  string? Search,
  int? MaxProposalCount,
  Guid? SpecilizationId,
  JobType? JobType,
  JobExperienceLevel? ExperienceLevel,
  BudgetRange? BudgetRange,
  HourlyRateRange? HourlyRateRange,
  List<Guid>? SkillIds,
  int Page = 1,
  int PageSize = 10,
  string SortBy = "newest"
) : ICachedQuery<Result<PaginatedList<JobSummaryDto>>>
{

  public string CacheKey
  {
    get
    {
      var skills = SkillIds?.OrderBy(id => id).Select(id => id.ToString("N")) ?? Enumerable.Empty<string>();
      var search = (Search ?? string.Empty).Trim().ToLowerInvariant();
      var sortBy = string.IsNullOrWhiteSpace(SortBy) ? "newest" : SortBy.Trim().ToLowerInvariant();

      return string.Join('|', new[]
      {
        "GetJobsQuery",
        $"search={search}",
        $"maxProposalCount={MaxProposalCount?.ToString() ?? "null"}",
        $"spec={SpecilizationId?.ToString("N") ?? "null"}",
        $"type={(int?)JobType ?? -1}",
        $"exp={(int?)ExperienceLevel ?? -1}",
        $"budget={(int?)BudgetRange ?? -1}",
        $"hourly={(int?)HourlyRateRange ?? -1}",
        $"skills={string.Join(',', skills)}",
        $"page={Page}",
        $"size={PageSize}",
        $"sort={sortBy}"
      });
    }
  }



  public string[] Tags => new[] { "jobs" };
}