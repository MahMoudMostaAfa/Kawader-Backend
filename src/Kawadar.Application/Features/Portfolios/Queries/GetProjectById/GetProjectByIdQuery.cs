using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectById
{
    public record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectDTO>>;
}
