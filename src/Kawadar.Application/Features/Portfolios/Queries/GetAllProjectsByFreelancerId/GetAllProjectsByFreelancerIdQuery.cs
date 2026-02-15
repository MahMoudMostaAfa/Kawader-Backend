using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId
{
    public record GetAllProjectsByFreelancerIdQuery(Guid Id) : IRequest<Result<List<ProjectDTO>>>;
}
