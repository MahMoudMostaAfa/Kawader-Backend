using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobReports
{
    public record GetJobReportsQuery(ReportType? reportType, ReportStatus? reportStatus,
        int Page = 1, int PageSize = 10, string SortBy = "newest") : IRequest<Result<PaginatedList<BriefJobReportDto>>>;
}
