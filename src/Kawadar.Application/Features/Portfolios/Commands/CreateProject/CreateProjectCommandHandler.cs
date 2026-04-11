using AutoMapper;
using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;
using Kawadar.Domain.Specilizations;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateProject
{
    public class CreateProjectCommandHandler(IUnitOfWork unitOfWork, IUser user, IPortfolioProjectRepository projectRepository
        , IUsersRepository usersRepository, ISpecilizationRepository specilizationRepository
        , IStorageClient storageClient, IMapper mapper) : IRequestHandler<CreateProjectCommand, Result<ProjectDTO>>
    {
        public async Task<Result<ProjectDTO>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (result.IsError) return result.Errors;

            var freelancer = result.Value;

            var previousProjects = await projectRepository.GetAllByFreelancerId(freelancer.Id);
            var displayOrder = previousProjects.Count() + 1;

            using var stream = request.ProjectImage.OpenReadStream();
            var uploadResult = await storageClient.UploadFileAsync(stream, request.ProjectImage.FileName,
                Containers.PortfolioProjects, cancellationToken);

            if (uploadResult.IsError) return uploadResult.Errors;

            var projectImageUrl = uploadResult.Value;
            var specilizationId = Guid.Empty;
            var SpecilizationResult = await specilizationRepository.GetByName(request.specilization);
            if (SpecilizationResult.IsError && SpecilizationResult.TopError.Type == ErrorKind.NotFound)
            {
                var newSpecilization = Specilization.Create(request.specilization, true);
                if (newSpecilization.IsError) return newSpecilization.Errors;
                var addResult = await specilizationRepository.AddAsync(newSpecilization.Value);
                if (addResult.IsError) return addResult.Errors;
                specilizationId = newSpecilization.Value.Id;
            }
            else if (SpecilizationResult.IsSuccess)
            { 
                specilizationId = SpecilizationResult.Value.Id;
            }

            else
            {
                return SpecilizationResult.Errors;
            }

            var resultProject = PortfolioProject.Create(request.Title, request.Description, specilizationId, freelancer.Id,
                projectImageUrl, displayOrder, request.ProjectUrl);

            if (resultProject.IsError) return resultProject.Errors;

            await projectRepository.AddAsync(resultProject.Value);
            var projectDTO = mapper.Map<ProjectDTO>(resultProject.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return projectDTO;
        }
    }
}
