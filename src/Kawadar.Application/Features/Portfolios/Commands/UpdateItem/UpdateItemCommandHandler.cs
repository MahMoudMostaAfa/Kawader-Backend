using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateItem
{
    public class UpdateItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IStorageClient storageClient) : IRequestHandler<UpdateItemCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetProjectItemById(request.Id);
            if (result.IsError) return result.Errors;

            var item = result.Value;
            string? content = null;

            if(item.ItemType == ItemType.Image)
            {
                var storageDeleteResult = await storageClient.DeleteFileAsync(item.Content, Containers.PortfolioProjectItems);

                if (storageDeleteResult.IsError) return storageDeleteResult.Errors;
            }

            if(request.itemType == ItemType.Image && request.Image is not null)
            {
                using var stream = request.Image.OpenReadStream();
                var uploadResult = await storageClient.UploadFileAsync(stream, request.Image.FileName, Containers.PortfolioProjectItems, cancellationToken);
                if (uploadResult.IsError) return uploadResult.Errors;

                content = uploadResult.Value;
            }
            else
            {
                content = request.Content;
            }

            var updateResult = item.Update(request.itemType, content!);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
