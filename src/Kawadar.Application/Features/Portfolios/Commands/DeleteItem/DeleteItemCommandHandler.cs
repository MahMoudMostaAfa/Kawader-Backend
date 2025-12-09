using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteItem
{
    public class DeleteItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<DeleteItemCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetProjectItemById(request.Id);

            if (result.IsError) return result.Errors;

            var deleteResult = projectRepository.DeleteItem(result.Value);

            if (deleteResult.IsError) return deleteResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return deleteResult;
        }
    }
}
