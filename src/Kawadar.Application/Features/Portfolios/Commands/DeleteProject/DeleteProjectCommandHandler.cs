using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteProject
{
    public class DeleteProjectCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IStorageClient storageClient) : IRequestHandler<DeleteProjectCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetPortfolioProjectById(request.Id);
            if (result.IsError) return result.Errors;

            var project = result.Value;

            var projects = await projectRepository.GetAllByFreelancerId(project.FreelancerId);
            foreach (var previousProject in projects)
            {
                if (previousProject.DisplayOrder > project.DisplayOrder)
                    previousProject.UpdateOrder(previousProject.DisplayOrder - 1);
            }

            if (project.ProjectImageUrl != string.Empty)
            {
                var storageDeleteResult = await storageClient.DeleteFileAsync(project.ProjectImageUrl, Containers.PortfolioProjects);
                if (storageDeleteResult.IsError) return storageDeleteResult.Errors;
            }

            var deleteResult = projectRepository.Delete(project);
            if (deleteResult.IsError) return deleteResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return deleteResult;
        }
    }
}
