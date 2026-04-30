using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Violations.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Violations.Queries.GetViolationById
{
    public class GetViolationByIdQueryHandler(IUser user, IViolationRepository violationRepository, IUsersRepository usersRepository
        , IIdentityService identityService, IMapper mapper) : IRequestHandler<GetViolationByIdQuery, Result<FullViolationDto>>
    {
        public async Task<Result<FullViolationDto>> Handle(GetViolationByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var ViolationResult = await violationRepository.GetViolationById(request.Id);
            if (ViolationResult.IsError) return ViolationResult.Errors;

            var violation = ViolationResult.Value;
            var ReportedUser = await usersRepository.GetUserProfileByIdAsync(violation.UserId);
            if (ReportedUser.IsError) return ReportedUser.Errors;

            var userDto = await identityService.GetUserByIdAsync(ReportedUser.Value.UserId);
            if (userDto.IsError) return userDto.Errors;

            var violationDto = mapper.Map<FullViolationDto>((violation, userDto.Value));
            if(violation.ResolvedBy is not null)
            {
                var adminUserProfile = await usersRepository.GetUserProfileByIdAsync((Guid)violation.ResolvedBy);
                if (adminUserProfile.IsError) return adminUserProfile.Errors;

                var adminDto = await identityService.GetUserByIdAsync(adminUserProfile.Value.UserId);
                if (adminDto.IsError) return adminDto.Errors;

                violationDto.ResolvedByUserName = adminDto.Value.UserName;
            }
            return violationDto;
        }
    }
}
