using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserReports
{
    public record GetUserReportsQuery(ReportType? reportType, ReportStatus? reportStatus, int page = 1
        , int pageSize = 10, string sortBy = "oldest") : IRequest<Result<PaginatedList<BriefUserReportDto>>>;
}
