using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.ReportJob
{
    public record ReportJobCommand(string slug, ReportType reportType, string content) : IRequest<Result<Success>>;
}