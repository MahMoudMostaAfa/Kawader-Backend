using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancers
{
    public class GetFreelancersQueryHandler(IUser user, IUsersRepository usersRepository
        , IIdentityService identityService, IMapper mapper) : IRequestHandler<GetFreelancersQuery, Result<PaginatedList<BriefFreelancerDto>>>
    {
        public async Task<Result<PaginatedList<BriefFreelancerDto>>> Handle(GetFreelancersQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfiles = await usersRepository.GetFreelancers(
                request.ExperienceYear,
                request.specilizationId,
                request.averageRating,
                request.page,
                request.pageSize,
                request.sortBy
                );
            var UserIds = userProfiles.Items.Select(x => x.UserId);

            var UserDtosResult = await identityService.GetUsersByIds(UserIds);
            if (UserDtosResult.IsError) return UserDtosResult.Errors;

            var UserProfileDtos = userProfiles.Items
                .Zip(UserDtosResult.Value, (profile, dto) => (profile, dto))
                .Select(pair => mapper.Map<BriefFreelancerDto>(pair))
                .ToList();

            return new PaginatedList<BriefFreelancerDto>(UserProfileDtos, userProfiles.TotalCount, request.page, request.pageSize);
        }
    }
}
