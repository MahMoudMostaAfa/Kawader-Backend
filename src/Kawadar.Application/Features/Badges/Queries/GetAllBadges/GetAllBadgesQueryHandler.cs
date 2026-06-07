using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Queries.GetAllBadges
{
    public class GetAllBadgesQueryHandler(IUser user, IMapper mapper,
        IBadgeRepository badgeRepository) : IRequestHandler<GetAllBadgesQuery, Result<List<BadgeDTO>>>
    {
        public async Task<Result<List<BadgeDTO>>> Handle(GetAllBadgesQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var badges = await badgeRepository.GetAllBadges();
            var badgeDtos = mapper.Map<List<BadgeDTO>>(badges);
            return badgeDtos;
        }
    }
}
