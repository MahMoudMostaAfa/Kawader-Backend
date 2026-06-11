using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;
using System.Globalization;

namespace Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug
{
    public record GetReportsByJobSlugQuery(string JobSlug, ReportStatus? status, ReportType? type, int page, int pageSize, string sortBy) : IRequest<Result<PaginatedList<BriefJobReportDto>>>, ICachedQuery
    {
        public string CacheKey => $"JobsReports-{JobSlug}-{type?.ToString() ?? "all"}-{status?.ToString() ?? "all"}-{page}-{pageSize}-{sortBy}";

        public string[] Tags => ["JobReports"];
    }
}
