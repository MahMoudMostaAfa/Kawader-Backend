
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Queries
{
    public class GetBadgeByIdQueryHandler(IUser user, IBadgeRepository badgeRepository) : IRequestHandler<GetBadgeByIdQuery, Result<BadgeDTO>>
    {
        public async Task<Result<BadgeDTO>> Handle(GetBadgeByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await badgeRepository.GetById(request.Id);
            if (result.IsError) return result.Errors;
            var badge = result.Value;
            return new BadgeDTO { Id = badge.Id, title = badge.Title, IconUrl = badge.IconUrl, description = badge.Description};
        }
    }
}
