using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateItem
{
    public class UpdateItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<UpdateItemCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetProjectItemById(request.Id);

            if (result.IsError) return result.Errors;

            var item = result.Value;
            var updateResult = item.Update(request.Content);

            if (updateResult.IsError) return updateResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return updateResult;
        }
    }
}
