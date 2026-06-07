using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserReports
{
    public class GetUserReportsQueryHandler(IUser user, IIdentityService identityService,
        IUsersRepository usersRepository, IMapper mapper) : IRequestHandler<GetUserReportsQuery, Result<PaginatedList<BriefUserReportDto>>>
    {
        public async Task<Result<PaginatedList<BriefUserReportDto>>> Handle(GetUserReportsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var reports = await usersRepository.GetUserReports(request.reportType, request.reportStatus, request.page, request.pageSize, request.sortBy);

            var reporterUserProfileIds = reports.Items.Select(x => x.ReportedBy);
            var reporterUserProfiles = await usersRepository.GetUsersbyIds(reporterUserProfileIds);
            if (reporterUserProfiles.IsError) return reporterUserProfiles.Errors;

            var reporterUserIds = reporterUserProfiles.Value.Select(x => x.UserId);
            var reporterUserDtos = await identityService.GetUsersByIds(reporterUserIds);
            if (reporterUserDtos.IsError) return reporterUserDtos.Errors;

            var reportedUserProfileIds = reports.Items.Select(x => x.ReportedUser);
            var reportedUserProfiles = await usersRepository.GetUsersbyIds(reportedUserProfileIds);
            if (reportedUserProfiles.IsError) return reportedUserProfiles.Errors;

            var reportedUserIds = reportedUserProfiles.Value.Select(x => x.UserId);
            var reportedUserDtos = await identityService.GetUsersByIds(reportedUserIds);
            if (reportedUserDtos.IsError) return reportedUserDtos.Errors;

            List<BriefUserReportDto> briefReports = new();
            for(int i = 0; i < reports.TotalCount; i++)
            {
                var briefReport = mapper.Map<BriefUserReportDto>((reports.Items[i], reporterUserDtos.Value.ElementAt(i), reportedUserDtos.Value.ElementAt(i)));
                briefReports.Add(briefReport);
            }

            return new PaginatedList<BriefUserReportDto>(briefReports, reports.TotalCount, request.page, request.pageSize);
        }
    }
}
