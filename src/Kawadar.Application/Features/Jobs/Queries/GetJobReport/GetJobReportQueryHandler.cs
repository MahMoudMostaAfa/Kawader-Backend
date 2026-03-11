using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobReport
{
    public class GetJobReportQueryHandler(IUser user ,IJobsRepository jobsRepository
        , IUsersRepository usersRepository, IIdentityService identityService, IMapper mapper) : IRequestHandler<GetJobReportQuery, Result<FullJobReportDto>>
    {
        public async Task<Result<FullJobReportDto>> Handle(GetJobReportQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userDto = await identityService.GetUserByIdAsync(userId);
            if (userDto.IsError) return userDto.Errors;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var JobReportResult = await jobsRepository.GetJobReportById(request.Id);
            if (JobReportResult.IsError) return JobReportResult.Errors;

            var JobResult = await jobsRepository.GetJobByIdAsync(JobReportResult.Value.JobId);
            if (JobResult.IsError) return JobResult.Errors;

            var ReportDto = mapper.Map<FullJobReportDto>((JobReportResult.Value, JobResult.Value, userDto.Value));
            return ReportDto;
        }
    }
}
