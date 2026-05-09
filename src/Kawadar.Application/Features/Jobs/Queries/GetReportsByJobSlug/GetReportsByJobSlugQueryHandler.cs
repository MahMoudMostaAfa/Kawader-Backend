using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug
{
    public class GetReportsByJobSlugQueryHandler(IUser user, IUsersRepository usersRepository,
        IJobsRepository jobsRepository, IIdentityService identityService, IMapper mapper) : IRequestHandler<GetReportsByJobSlugQuery, Result<PaginatedList<BriefJobReportDto>>>
    {
        public async Task<Result<PaginatedList<BriefJobReportDto>>> Handle(GetReportsByJobSlugQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var Reports = await jobsRepository.GetReportsByJobSlug(request.JobSlug, request.type, request.status, request.page, request.pageSize, request.sortBy);

            var reporterIds = Reports.Value.Items.Select(x => x.ReportedBy);
            var UserProfiles = await usersRepository.GetUsersbyIds(reporterIds);
            if (UserProfiles.IsError) return UserProfiles.Errors;

            var UserIds = UserProfiles.Value.Select(x => x.UserId);
            var UserDtos = await identityService.GetUsersByIds(UserIds);
            if (UserDtos.IsError) return UserDtos.Errors;

            var JobsIds = Reports.Value.Items.Select(x => x.JobId);
            var JobsResult = await jobsRepository.GetJobsByIds(JobsIds);
            if (JobsResult.IsError) return JobsResult.Errors;

            List<BriefJobReportDto> jobReports = new();
            for (var i = 0; i < Reports.Value.TotalCount; i++)
            {
                var briefJobReport = mapper.Map<BriefJobReportDto>((Reports.Value.Items[i], JobsResult.Value.ElementAt(i), UserDtos.Value.ElementAt(i)));
                jobReports.Add(briefJobReport);
            }

            return new PaginatedList<BriefJobReportDto>(jobReports, Reports.Value.TotalCount, request.page, request.pageSize);
        }
    }
}