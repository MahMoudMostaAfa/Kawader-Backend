using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetAdmins
{
    public class GetAdminsQueryHandler(IUser user, IUsersRepository usersRepository
        , IIdentityService identityService, IMapper mapper) : IRequestHandler<GetAdminsQuery, Result<List<AdminDto>>>
    {
        public async Task<Result<List<AdminDto>>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var admins = await usersRepository.GetAdmins();
            var adminIds = admins.Select(x => x.UserId);

            var userDtosResult = await identityService.GetUsersByIds(adminIds);
            if (userDtosResult.IsError) return userDtosResult.Errors;

            var AdminDtos = admins
                .Zip(userDtosResult.Value, (profile, dto) => (profile, dto))
                .Select(pair => mapper.Map<AdminDto>(pair))
                .ToList();

            return AdminDtos;
        }
    }
}
