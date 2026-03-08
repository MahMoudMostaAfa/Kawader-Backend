using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobReport
{
    public record GetJobReportQuery(Guid Id) : IRequest<Result<FullJobReportDto>>;
}
