using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.StorageRepository;
using MediatR;
using System.Runtime.Versioning;

namespace Kawadar.Application.Features.Badges.Commands.UpdateBadge
{
    public class UpdateBadgeCommandHandler(IUnitOfWork unitOfWork, IUser user,
        IBadgeRepository badgeRepository, IStorageClient storageClient) : IRequestHandler<UpdateBadgeCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBadgeCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await badgeRepository.GetById(request.badgeId);
            if (result.IsError) return result.Errors;
            var badge = result.Value;

            var deleteResult = await storageClient.DeleteFileAsync(badge.IconUrl, Containers.Badges);
            if (deleteResult.IsError) return deleteResult.Errors;

            using var stream = request.Icon.OpenReadStream();
            var uploadResult = await storageClient.UploadFileAsync(stream, request.Icon.FileName, Containers.Badges, cancellationToken);

            if (uploadResult.IsError) return uploadResult.Errors;

            badge.Update(uploadResult.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Updated;
        }
    }
}
