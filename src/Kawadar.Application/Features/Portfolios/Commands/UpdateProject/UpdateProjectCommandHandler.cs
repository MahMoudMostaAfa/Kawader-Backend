using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateProject
{
    public class UpdateProjectCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<UpdateProjectCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetPortfolioProjectById(request.Id);

            if (result.IsError) return result.Errors;

            var project = result.Value;

            var updateResult = project.Update(request.ProjectUrl, request.ImageUrl, request.DisplayOrder, request.IsPublic);

            if (updateResult.IsError) return updateResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return updateResult;
        }
    }
}
