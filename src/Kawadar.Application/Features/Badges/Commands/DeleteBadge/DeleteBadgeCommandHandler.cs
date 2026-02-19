using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using MediatR;


namespace Kawadar.Application.Features.Badges.Commands.DeleteBadge
{
    public class DeleteBadgeCommandHandler(IUnitOfWork unitOfWork,IUser user, IBadgeRepository badgeRepository, IStorageClient storageClient) : IRequestHandler<DeleteBadgeCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteBadgeCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await badgeRepository.GetById(request.badgeId);
            if (result.IsError) return result.Errors;
            var badge = result.Value;
            var storageDeleteResult = await storageClient.DeleteFileAsync(badge.IconUrl, Containers.Badges);

            if (storageDeleteResult.IsError) return storageDeleteResult.Errors;

            var deleteResult = badgeRepository.Delete(badge);
            if (deleteResult.IsError) return deleteResult.Errors;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return deleteResult;
        }
    }
}
