using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteProject
{
    public class DeleteProjectCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<DeleteProjectCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetPortfolioProjectById(request.Id);
            if (result.IsError) return result.Errors;

            var deleteResult = projectRepository.Delete(result.Value);
            if (deleteResult.IsError) return deleteResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return deleteResult;
        }
    }
}
