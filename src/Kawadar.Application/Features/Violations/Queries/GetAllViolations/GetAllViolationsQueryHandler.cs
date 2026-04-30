using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Violations.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Violations.Queries.GetAllViolations
{
    public class GetAllViolationsQueryHandler(IUser user , IUsersRepository usersRepository,
        IIdentityService identityService, IViolationRepository violationRepository, IMapper mapper) : IRequestHandler<GetAllViolationsQuery, Result<PaginatedList<BriefViolationDto>>>
    {
        public async Task<Result<PaginatedList<BriefViolationDto>>> Handle(GetAllViolationsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var violations = await violationRepository.GetAllViolation(request.status, request.type, request.page, request.pageSize, request.sortBy);
            var userIds = violations.Items.Select(x => x.UserId);

            var UserProfiles = await usersRepository.GetUsersbyIds(userIds);
            if (UserProfiles.IsError) return UserProfiles.Errors;

            var IdentityUsersIds = UserProfiles.Value.Select(x => x.UserId);
            var userDtos = await identityService.GetUsersByIds(IdentityUsersIds);
            if (userDtos.IsError) return userDtos.Errors;

            var BriefViolations = violations.Items
                .Zip(userDtos.Value, (violation, userDto) => (violation, userDto))
                .Select(pair => mapper.Map<BriefViolationDto>(pair))
                .ToList();

            return new PaginatedList<BriefViolationDto>(BriefViolations, violations.TotalCount, request.page, request.pageSize);
        }
    }
}
