using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateUserReport
{
    public record UpdateUserReportCommand(Guid reportId, ReportStatus ReportStatus, string ActionTaken) : IRequest<Result<Updated>>;
}
