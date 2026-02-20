using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateImageItem
{
    public class UpdateItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IStorageClient storageClient) : IRequestHandler<UpdateImageItemCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateImageItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectRepository.GetProjectItemById(request.Id);

            if (result.IsError) return result.Errors;

            var item = result.Value;
            var storageDeleteResult = await storageClient.DeleteFileAsync(item.Content, Containers.PortfolioProjectItems);

            if (storageDeleteResult.IsError) return storageDeleteResult.Errors;

            using var stream = request.Image.OpenReadStream();
            var uploadResult = await storageClient.UploadFileAsync(stream, request.Image.FileName, Containers.PortfolioProjectItems, cancellationToken);
            if (uploadResult.IsError) return uploadResult.Errors;

            var imageUrl = uploadResult.Value;
            var updateResult = item.Update(imageUrl);

            if (updateResult.IsError) return updateResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return updateResult;
        }
    }
}
