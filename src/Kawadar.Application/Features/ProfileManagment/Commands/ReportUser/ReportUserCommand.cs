using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.ReportUser
{
    public record ReportUserCommand(string ReportedUserName, string content, ReportType reportType) : IRequest<Result<Created>>;
}
