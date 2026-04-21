using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserReportByUserName
{
    public class GetUserReportsByUserNameQueryHandler(IUser user, IUsersRepository usersRepository
        , IIdentityService identityService, IMapper mapper) : IRequestHandler<GetUserReportByUserNameQuery, Result<PaginatedList<BriefUserReportDto>>>
    {
        public async Task<Result<PaginatedList<BriefUserReportDto>>> Handle(GetUserReportByUserNameQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var ReportedUserDto = await identityService.GetUserByUserNameAsync(request.userName);
            if (ReportedUserDto.IsError) return ReportedUserDto.Errors;

            var reportedUserProfile = await usersRepository.GetUserProfileByUserIdAsync(ReportedUserDto.Value.Id);
            if (reportedUserProfile.IsError) return reportedUserProfile.Errors;

            var reports = await usersRepository.GetUserReportsByUserId(reportedUserProfile.Value.Id, request.reportStatus
                , request.reportType, request.page, request.pageSize, request.sortBy);

            var reporterUserProfileIds = reports.Items.Select(x => x.ReportedBy);
            var reporterUserProfiles = await usersRepository.GetUsersbyIds(reporterUserProfileIds);
            if (reporterUserProfiles.IsError) return reporterUserProfiles.Errors;

            var reporterUserIds = reporterUserProfiles.Value.Select(x => x.UserId);
            var reporterUserDtos = await identityService.GetUsersByIds(reporterUserIds);
            if (reporterUserDtos.IsError) return reporterUserDtos.Errors;

            List<BriefUserReportDto> briefReports = new();
            for (int i = 0; i < reports.TotalCount; i++)
            {
                var briefReport = mapper.Map<BriefUserReportDto>((reports.Items[i], reporterUserDtos.Value.ElementAt(i), ReportedUserDto.Value));
                briefReports.Add(briefReport);
            }

            return new PaginatedList<BriefUserReportDto>(briefReports, reports.TotalCount, request.page, request.pageSize);
        }
    }
}
