using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobReport
{
    public record UpdateJobReportCommand(Guid Id, string ActionTaken, ReportStatus reportStatus) : IRequest<Result<Updated>>;
}
