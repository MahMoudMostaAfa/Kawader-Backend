using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbuteById
{
    public class GetDisbuteByIdQueryHandler(IUser user, IDisbuteRepository disbuteRepository,
        IMapper mapper, IIdentityService identityService, IUsersRepository usersRepository) : IRequestHandler<GetDisbuteByIdQuery, Result<fullDisbuteDto>>
    {
        public async Task<Result<fullDisbuteDto>> Handle(GetDisbuteByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var disbuteResult = await disbuteRepository.GetDisbuteById(request.Id);
            if (disbuteResult.IsError) return disbuteResult.Errors;

            var userProfileResult = await usersRepository.GetUserProfileByIdAsync(disbuteResult.Value.RaisedById);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var userDtoResult = await identityService.GetUserByIdAsync(userProfileResult.Value.UserId);
            if (userDtoResult.IsError) return userDtoResult.Errors;

            var disbuteDto = mapper.Map<fullDisbuteDto>((disbuteResult.Value, userDtoResult.Value));
            return disbuteDto;
        }
    }
}
