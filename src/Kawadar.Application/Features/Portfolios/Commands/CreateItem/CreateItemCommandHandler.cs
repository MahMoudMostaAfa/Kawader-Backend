
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateItem
{
    public class CreateItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<CreateItemCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = PortfolioItem.Create(request.ItemType, request.Content,
                request.DisplayOrder, request.PortfolioProjectId);

            if (result.IsError) return result.Errors;

            var Item = result.Value;
            var addResult = await projectRepository.AddItemAsync(Item);

            if (addResult.IsError) return addResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return addResult;

        }
    }
}
