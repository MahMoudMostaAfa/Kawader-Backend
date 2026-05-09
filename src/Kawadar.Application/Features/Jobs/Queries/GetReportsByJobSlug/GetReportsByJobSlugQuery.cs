using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug
{
    public record GetReportsByJobSlugQuery(string JobSlug, ReportStatus? status, ReportType? type, int page, int pageSize, string sortBy) : IRequest<Result<PaginatedList<BriefJobReportDto>>>;
}
