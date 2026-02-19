using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.StorageRepository;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateProject
{
    public class UpdateProjectCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IStorageClient storageClient) : IRequestHandler<UpdateProjectCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetPortfolioProjectById(request.Id);

            if (result.IsError) return result.Errors;

            var project = result.Value;

            var ImageUrl = string.Empty;
            var file = request.Image;

            var deleteResult = await storageClient.DeleteFileAsync(project.ProjectImageUrl, Containers.PortfolioProjects);
            if (deleteResult.IsError) return deleteResult.Errors;

            using var stream = file.OpenReadStream();
            var uploadResult = await storageClient.UploadFileAsync(stream, file.FileName, Containers.PortfolioProjects, cancellationToken);
            if (uploadResult.IsError) return uploadResult.Errors;
            ImageUrl = uploadResult.Value;

            var updateResult = project.Update(request.ProjectUrl, ImageUrl, request.IsPublic);

            if (updateResult.IsError) return updateResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return updateResult;
        }
    }
}