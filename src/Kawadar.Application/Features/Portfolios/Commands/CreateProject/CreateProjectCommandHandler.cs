using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Mapper;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateProject
{
    public class CreateProjectCommandHandler(IUnitOfWork unitOfWork, IUser user, IPortfolioProjectRepository projectRepository
        , IUsersRepository usersRepository) : IRequestHandler<CreateProjectCommand, Result<ProjectDTO>>
    {
        public async Task<Result<ProjectDTO>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (result.IsError) return result.Errors;

            var freelancer = result.Value;
            var resultProject = PortfolioProject.Create(request.Title, request.Description, request.Category, freelancer.Id);
            if (resultProject.IsError) return resultProject.Errors;

            await projectRepository.AddAsync(resultProject.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return resultProject.Value.toDTO();
        }
    }
}
