using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId
{
    public class GetAllProjectsByFreelancerUserNameHandler(IUser user, IPortfolioProjectRepository projectRepository, 
        IUsersRepository usersRepository, IIdentityService identityService, IMapper mapper) : IRequestHandler<GetAllProjectsByFreelancerUserNameQuery, Result<List<ProjectDTO>>>
    {
        public async Task<Result<List<ProjectDTO>>> Handle(GetAllProjectsByFreelancerUserNameQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var UserDtoResult = await identityService.GetUserByUserNameAsync(request.UserName);
            if (UserDtoResult.IsError) return UserDtoResult.Errors;

            var freelancerProfileResult = await usersRepository.GetUserProfileByUserIdAsync(UserDtoResult.Value.Id);
            if (freelancerProfileResult.IsError) return freelancerProfileResult.Errors;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;
            var userProfileId = userProfileResult.Value.Id;

            var projects = await projectRepository.GetAllByFreelancerId(freelancerProfileResult.Value.Id);

            if(userProfileId == freelancerProfileResult.Value.Id)
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