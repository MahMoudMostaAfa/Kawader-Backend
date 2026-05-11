using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetRecommendationJobs;

public record GetRecommandationJobsQuery(int page = 1, int pageSize = 10) : IRequest<Result<PaginatedList<JobSummaryDto>>>;