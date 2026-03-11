using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.ReportJob
{
    public class ReportJobCommandHandler(IUser user, IUsersRepository usersRepository
        , IJobsRepository jobsRepository, IUnitOfWork unitOfWork) : IRequestHandler<ReportJobCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(ReportJobCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var JobResult = await jobsRepository.GetJobBySlugAsync(request.slug);
            if (JobResult.IsError) return JobResult.Errors;

            var jobReportResult = JobReport.Create(JobResult.Value.Id, userProfileResult.Value.Id, request.content, request.reportType);
            if (jobReportResult.IsError) return jobReportResult.Errors;

            await jobsRepository.AddJobReport(jobReportResult.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}