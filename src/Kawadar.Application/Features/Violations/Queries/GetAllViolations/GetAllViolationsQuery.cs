using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Violations.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Violations.Enums;
using MediatR;

namespace Kawadar.Application.Features.Violations.Queries.GetAllViolations
{
    public record GetAllViolationsQuery(ViolationStatus? status, ViolationType? type
        , int page = 1, int pageSize = 10, string sortBy = "newest") : IRequest<Result<PaginatedList<BriefViolationDto>>>;
}
