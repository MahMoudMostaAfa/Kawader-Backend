using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Queries.GetFreelancerBadgesQuery
{
    public class GetFreelancerBadgesHandler(IUser user, IBadgeRepository badgeRepository
        , IUsersRepository usersRepository, IMapper mapper) : IRequestHandler<GetFreelancerBadgesQuery, Result<List<BadgeDTO>>>
    {
        public async Task<Result<List<BadgeDTO>>> Handle(GetFreelancerBadgesQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var UserProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (UserProfileResult.IsError) return UserProfileResult.Errors;

            var badges = await badgeRepository.GetAllFreelancerBadges(UserProfileResult.Value.Id);
            var badgesDTO = mapper.Map<List<BadgeDTO>>(badges);
            return badgesDTO;

        }
    }
}
