using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobs;

public record GetJobsQuery(
  string? Search,
  Guid? SpecilizationId,
  JobType? JobType,
  JobExperienceLevel? ExperienceLevel,
  BudgetRange? BudgetRange,
  HourlyRateRange? HourlyRateRange,
  List<Guid>? SkillIds,
  int Page = 1,
  int PageSize = 10,
  string SortBy = "newest"
) : IRequest<Result<PaginatedList<JobSummaryDto>>>;
