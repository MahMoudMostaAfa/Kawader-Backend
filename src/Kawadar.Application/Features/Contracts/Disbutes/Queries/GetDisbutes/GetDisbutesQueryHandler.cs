using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbutes
{
    public class GetDisbutesQueryHandler(IUser user, IDisbuteRepository disbuteRepository
        , IUsersRepository usersRepository, IIdentityService identityService, IMapper mapper) : IRequestHandler<GetDisbutesQuery, Result<PaginatedList<BriefDisbuteDto>>>
    {
        public async Task<Result<PaginatedList<BriefDisbuteDto>>> Handle(GetDisbutesQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var disbutes = await disbuteRepository.GetAllDisbutes(request.status, request.Page, request.PageSize, request.SortBy);
            var totalCount = disbutes.TotalCount;

            var UserProfilesIds = disbutes.Items.Select(x => x.RaisedById);
            var userProfilesResult = await usersRepository.GetUsersbyIds(UserProfilesIds);
            if (userProfilesResult.IsError) return userProfilesResult.Errors;

            var IdentityUsersIds = userProfilesResult.Value.Select(x => x.UserId);
            var userDtos = await identityService.GetUsersByIds(IdentityUsersIds);
            if (userDtos.IsError) return userDtos.Errors;

            var disbutesDto = disbutes.Items
                .Zip(userDtos.Value, (disbute, userDto) => (disbute, userDto))
                .Select(pair => mapper.Map<BriefDisbuteDto>(pair))
                .ToList();

            return new PaginatedList<BriefDisbuteDto>(disbutesDto, totalCount, request.Page, request.PageSize);
        }
    }
}
