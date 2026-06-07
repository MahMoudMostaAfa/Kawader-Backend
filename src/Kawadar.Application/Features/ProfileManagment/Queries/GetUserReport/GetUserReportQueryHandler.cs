using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserReport
{
    public class GetUserReportQueryHandler(IUser user, IUsersRepository usersRepository,
        IIdentityService identityService, IMapper mapper) : IRequestHandler<GetUserReportQuery, Result<FullUserReportDto>>
    {
        public async Task<Result<FullUserReportDto>> Handle(GetUserReportQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var report = await usersRepository.GetUserReportById(request.reportId);
            if (report.IsError) return report.Errors;

            var reporterId = report.Value.ReportedBy;
            var reporterUserProfile = await usersRepository.GetUserProfileByIdAsync(reporterId);
            if (reporterUserProfile.IsError) return reporterUserProfile.Errors;

            var reporterUserDto = await identityService.GetUserByIdAsync(reporterUserProfile.Value.UserId);
            if (reporterUserDto.IsError) return reporterUserDto.Errors;

            var reportedId = report.Value.ReportedUser;
            var reportedUserProfile = await usersRepository.GetUserProfileByIdAsync(reportedId);
            if (reportedUserProfile.IsError) return reportedUserProfile.Errors;

            var reportedUserDto = await identityService.GetUserByIdAsync(reportedUserProfile.Value.UserId);
            if (reportedUserDto.IsError) return reportedUserDto.Errors;

            var fullReport = mapper.Map<FullUserReportDto>((report.Value, reporterUserDto.Value, reportedUserDto.Value));
            return fullReport;
        }
    }
}
