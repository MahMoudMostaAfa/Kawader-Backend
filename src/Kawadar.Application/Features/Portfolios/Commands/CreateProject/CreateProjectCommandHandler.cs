using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Mapper;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.StorageRepository;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateProject
{
    public class CreateProjectCommandHandler(IUnitOfWork unitOfWork, IUser user, IPortfolioProjectRepository projectRepository
        , IUsersRepository usersRepository, IStorageClient storageClient) : IRequestHandler<CreateProjectCommand, Result<ProjectDTO>>
    {
        public async Task<Result<ProjectDTO>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (result.IsError) return result.Errors;

            var freelancer = result.Value;
            var projectImageUrl = "";
            if(request.ProjectImage is not null)
            {
                using var stream = request.ProjectImage.OpenReadStream();
                var uploadResult = await storageClient.UploadFileAsync(stream, request.ProjectImage.FileName,
                    Containers.PortfolioProjects, cancellationToken);

                if (uploadResult.IsError) return uploadResult.Errors;
                projectImageUrl = uploadResult.Value;
            }
            var resultProject = PortfolioProject.Create(request.Title, request.Description, request.Category, freelancer.Id,
                projectImageUrl, request.ProjectUrl);
            if (resultProject.IsError) return resultProject.Errors;

            await projectRepository.AddAsync(resultProject.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return resultProject.Value.toDTO();
        }
    }
}
