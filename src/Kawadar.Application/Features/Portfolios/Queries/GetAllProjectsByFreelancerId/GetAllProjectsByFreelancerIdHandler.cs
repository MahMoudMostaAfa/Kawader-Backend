using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId
{
    public class GetAllProjectsByFreelancerIdHandler(IUser user, IPortfolioProjectRepository projectRepository, 
        IUsersRepository usersRepository, IMapper mapper) : IRequestHandler<GetAllProjectsByFreelancerIdQuery, Result<List<ProjectDTO>>>
    {
        public async Task<Result<List<ProjectDTO>>> Handle(GetAllProjectsByFreelancerIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;
            var userProfileId = userProfileResult.Value.Id;

            var projects = await projectRepository.GetAllByFreelancerId(request.Id);

            if(userProfileId == request.Id)
            {
                var projectsDTO = mapper.Map<List<ProjectDTO>>(projects);
                return projectsDTO;
            }
            else
            {
                var publicProjects = projects.Where(x => x.IsPublic == true);
                var projectsDTO = mapper.Map<List<ProjectDTO>>(publicProjects);
                return projectsDTO;
            }
            
        }
    }
}