using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Commands.UpdateBadge
{
    public class UpdateBadgeCommandHandler(IUnitOfWork unitOfWork, IUser user, IBadgeRepository badgeRepository) : IRequestHandler<UpdateBadgeCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBadgeCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await badgeRepository.GetById(request.badgeId);
            if (result.IsError) return result.Errors;
            var badge = result.Value;
            badge.Update(request.IconUrl);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Updated;
        }
    }
}
