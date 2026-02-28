using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetUsers
{
    public class GetUserProfilesQueryHandler(IUser user, IUsersRepository usersRepository
        , IIdentityService identityService, IMapper mapper) : IRequestHandler<GetUserProfilesQuery, Result<List<UserProfileDto>>>
    {
        public async Task<Result<List<UserProfileDto>>> Handle(GetUserProfilesQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfiles = await usersRepository.GetUsers();
            var UserIds = userProfiles.Select(x => x.UserId);

            var UserDtosResult = await identityService.GetUsersByIds(UserIds);
            if (UserDtosResult.IsError) return UserDtosResult.Errors;

            var UserProfileDtos = userProfiles
                .Zip(UserDtosResult.Value, (profile, dto) => (profile, dto))
                .Select(pair => mapper.Map<UserProfileDto>(pair))
                .ToList();

            return UserProfileDtos;
        }
    }
}
