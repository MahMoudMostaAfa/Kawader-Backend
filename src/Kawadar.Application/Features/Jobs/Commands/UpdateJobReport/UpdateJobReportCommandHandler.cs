using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobReport
{
    public class UpdateJobReportCommandHandler(IUser user, IUsersRepository usersRepository
        , IJobsRepository jobsRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateJobReportCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateJobReportCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var jobReportResult = await jobsRepository.GetJobReportById(request.Id);
            if (jobReportResult.IsError) return jobReportResult.Errors;

            jobReportResult.Value.Update(request.reportStatus, request.ActionTaken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Updated;
        }
    }
}
