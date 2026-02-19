using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using MediatR;


namespace Kawadar.Application.Features.Badges.Commands.CreateBadge
{
    public class CreateBadgeCommandHandler(IUnitOfWork unitOfWork,IUser user, IBadgeRepository badgeRepository, IStorageClient storageClient) : IRequestHandler<CreateBadgeCommand, Result<BadgeDTO>>
    {
        public async Task<Result<BadgeDTO>> Handle(CreateBadgeCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            using var stream = request.Icon.OpenReadStream();
            var uploadResult = await storageClient.UploadFileAsync(stream, request.Icon.FileName, Containers.Badges, cancellationToken);

            if (uploadResult.IsError) return uploadResult.Errors;

            var result = Badge.Create(request.title, uploadResult.Value, request.description);

            if (result.IsError) return result.Errors;

            await badgeRepository.AddAsync(result.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new BadgeDTO { Id = result.Value.Id, title = result.Value.Title, description = result.Value.Description, IconUrl = result.Value.IconUrl};
        }
    }
}