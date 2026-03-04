using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId
{
    public record GetAllProjectsByFreelancerUserNameQuery(string UserName) : IRequest<Result<List<ProjectDTO>>>;
}
