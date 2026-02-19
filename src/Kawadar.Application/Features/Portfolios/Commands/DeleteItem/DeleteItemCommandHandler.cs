using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.StorageRepository;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;
using Kawadar.Application.Common.Constants;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteItem
{
    public class DeleteItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IStorageClient storageClient) : IRequestHandler<DeleteItemCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetProjectItemById(request.Id);

            if (result.IsError) return result.Errors;

            var item = result.Value;

            var projectItems = await projectRepository.GetProjectItemsByProjectId(item.PortfolioProjectId);

            foreach(var previousItem in projectItems)
            {
                if(previousItem.DisplayOrder > item.DisplayOrder)
                    previousItem.UpdateDisplayOrder(previousItem.DisplayOrder - 1);
            }

            if (item.ItemType == ItemType.Image)
            {
                var deleteStorageResult = await storageClient.DeleteFileAsync(item.Content, Containers.PortfolioProjectItems);
                if (deleteStorageResult.IsError) return deleteStorageResult.Errors;
            }

            var deleteResult = projectRepository.DeleteItem(item);

            if (deleteResult.IsError) return deleteResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return deleteResult;
        }
    }
}
