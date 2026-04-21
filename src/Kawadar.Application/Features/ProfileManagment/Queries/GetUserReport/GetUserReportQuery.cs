using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserReport
{
    public record GetUserReportQuery(Guid reportId) : IRequest<Result<FullUserReportDto>>;
}
