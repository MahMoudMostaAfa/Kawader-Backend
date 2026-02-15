using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Mapper;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId
{
    public class GetAllProjectsByFreelancerIdHandler(IUser user,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<GetAllProjectsByFreelancerIdQuery, Result<List<ProjectDTO>>>
    {
        public async Task<Result<List<ProjectDTO>>> Handle(GetAllProjectsByFreelancerIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var projects = await projectRepository.GetAllByFreelancerId(request.Id);

            var projectsDTO = projects.toDTOList();
            return projectsDTO;
        }
    }
}
