using Kawadar.Application.Features.Violations.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Violations.Queries.GetViolationById
{
    public record GetViolationByIdQuery(Guid Id) : IRequest<Result<FullViolationDto>>;
}
