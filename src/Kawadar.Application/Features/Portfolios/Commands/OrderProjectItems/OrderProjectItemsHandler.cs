using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.OrderProjectItems
{
    public class OrderProjectItemsHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository) : IRequestHandler<OrderProjectItemsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(OrderProjectItemsCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var items = await projectRepository.GetProjectItemsByProjectId(request.ProjectId);

            var itemsDictionary = items.ToDictionary(x => x.Id);
            var orderDictionary = request.Order.ToDictionary(x => x.Id, x => x.DisplayOrder);

            foreach(var order in orderDictionary)
            {
                if(!itemsDictionary.TryGetValue(order.Key, out PortfolioItem value))
                    return Error.NotFound("Item.NotFound", "Item not found");

                var item = itemsDictionary[order.Key];
                var updateResult = item.UpdateDisplayOrder(order.Value);
                if (updateResult.IsError) return updateResult.Errors;
            }

            await unitOfWork.SaveChangesAsync();
            return Result.Updated;
        }
    }
}